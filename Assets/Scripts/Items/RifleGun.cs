using System;
using System.Collections;
using System.Linq;
using TreeEditor;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static EventManager;

public class RifleGun : NetworkBehaviour, IItem
{
    private static readonly int TICK_PER_UPDATE = 2;                // Tick per update for vector
    private static readonly float GLOBAL_RECOIL_RESISTANCE = 100;   // Only change when you want to change rotation speed for other gun too
    private static readonly float GLOBAL_ROTATE_SPEED = 1000;       // Only change when you want to change rotation speed for other gun too

    public string Id => stats.Id;                                   // Id so it can be used in AvailableItemSO
    public string GunName => stats.GunName;                         // Variables for Inventory and UIs to read
    public Sprite Icon => stats.Icon;                               // Variables for Inventory and UIs to read
    public int CurrentAmmo => currentAmmo;                          // Variables for Inventory and UIs to read
    public Sprite AmmoIcon => stats.AmmoIcon;                       // Variables for Inventory and UIs to read
    public Sprite EmptyAmmoIcon => stats.EmptyAmmoIcon;             // Variables for Inventory and UIs to read
    public int MagazineSize => stats.MagazineSize;                  // Variables for Inventory and UIs to read
    public int Capacity => stats.Capacity;                          // Variables for Inventory and UIs to read
    public NetworkObject WeaponNetworkObject => networkObject;      // Variables for Inventory and UIs to read


    [SerializeField] private GunStats stats;                        // Stats for guns (this script would be use for rifles GameObject)
    [SerializeField] NetworkObject networkObject;                   // Just for external variable up there (WeaponNetworkObject)
    [SerializeField] SpriteRenderer weaponSpriteRenderer;           // Sprite render so we can flip it
    
    // Owner's mouse to player
    private readonly NetworkVariable<Vector2> mouseToPlayerVector = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkObject owner;                                    // Owner of the gun
    private ulong ownerID;                                          // ClientId of owner in ulong
    private int currentAmmo;                                        // Current Ammo before reload
    private bool isReloading;                                       // Inner variable so we know when the gun is reloading (can't shoot)
    private bool isOnFireRateCooldown;                              // Inner variable so we know when to shoot
    private bool isPickedUp;                                        // Inner variable so that gun can rotate
    private bool isShooting;                                        // Inner variable so we know left click is down
    private int tickCounter;                                        // Counter to update mouse to player vector

    void OnValidate()
    {
        networkObject = GetComponent<NetworkObject>();
        if (weaponSpriteRenderer != null)
            weaponSpriteRenderer.sprite = stats.WeaponSprite;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (!isPickedUp)
            return;
        EventManager.EventHandler.OnItemLeftClickPressedEvent -= OnLeftClickPressed;
        EventManager.EventHandler.OnItemLeftClickReleasesedEvent -= OnLeftClickReleased;
        EventManager.EventHandler.OnitemReloadEvent -= OnReloadPressed;
        EventManager.EventHandler.OnItemSwappedEvent -= OnSwapIn;
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
            currentAmmo = UnityEngine.Random.Range(0, stats.MagazineSize);
    }
    public override void OnNetworkDespawn()
    {
        if (isPickedUp)
            NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
        base.OnNetworkDespawn();
    }

    void UpdateMouseToPlayerVector()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Finding the closest player? Look like a working as a magnet
        // target = targetFinder.GetClosest();
        // moveDirection = (Vector2)target.position - (Vector2)characterPos.position;
        // //Debug.DrawRay(transform.position, targetWorldPos, Color.red, 0.01f);

        // vector from this object towards the target location
        mouseToPlayerVector.Value = mouseWorldPos - (Vector2)transform.position;
    }

    void Update()
    {
        if (!isPickedUp || !IsServer)
            return;
        // rotate that vector by 90 degrees around the Z axis
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * mouseToPlayerVector.Value;

        float singleStep = GLOBAL_ROTATE_SPEED * Time.deltaTime / stats.Weight;

        // get the rotation that points the Z axis forward, and the Y axis 90 degrees away from the target
        // (resulting in the X axis facing the target)
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, singleStep);


        if (transform.eulerAngles.z < 90 || transform.eulerAngles.z > 270)
        {
            weaponSpriteRenderer.flipY = false;
        }
        else
        {
            weaponSpriteRenderer.flipY = true;
        }

        if (isShooting)
            Shoot();
    }

    public void Shoot()
    {
        if (isReloading || isOnFireRateCooldown)
            return;

        if (currentAmmo == 0)
        {
            Reload();
            return;
        }

        // create & shoot the projectile 
        SpawnBullets(transform.rotation);
        StartCoroutine(FireRateCooldown());
        int previousValue = currentAmmo;
        currentAmmo -= 1;
        AudioManager.Instance.PlaySFX(stats.GunShotSFXs[UnityEngine.Random.Range(0, stats.GunShotSFXs.Length)], UnityEngine.Random.Range(0.7f, 1.1f));

        HandleRecoil();

        AmmoChangedEventArgs ammoChangedEventArgs = new()
        {
            OwnerID = ownerID,
            Item = this,
            PreviousValue = previousValue,
            CurrentValue = currentAmmo
        };
        EventManager.EventHandler.OnAmmoChanged(ammoChangedEventArgs);
    }

    private void Reload()
    {
        if (isReloading || currentAmmo == stats.MagazineSize)
            return;
        AudioManager.Instance.PlaySFX(stats.ReloadSFXs[UnityEngine.Random.Range(0, stats.ReloadSFXs.Length)], UnityEngine.Random.Range(0.7f, 1.1f));
        StartCoroutine(Reloading());
    }

    void HandleRecoil()
    {
        float thetaInDeg = transform.eulerAngles.z;
        float thetaInRadian = Mathf.Deg2Rad * thetaInDeg;
        float xOffset = transform.position.x + stats.BulletSpawnOffset * Mathf.Cos(thetaInRadian);
        float yOffset = transform.position.y + stats.BulletSpawnOffset * Mathf.Sin(thetaInRadian);
        Vector2 bulletOffset = new(xOffset, yOffset);
        Vector2 playerPos = owner.transform.position;

        Vector2 fromBulletToPlayer = playerPos - bulletOffset;
        Vector2 recoilVector = fromBulletToPlayer.normalized * stats.Recoil / GLOBAL_RECOIL_RESISTANCE;
        owner.transform.position = playerPos + recoilVector;
    }

    IEnumerator FireRateCooldown()
    {
        isOnFireRateCooldown = true;
        yield return new WaitForSecondsRealtime(stats.FireRate);
        isOnFireRateCooldown = false;
    }

    IEnumerator Reloading()
    {
        isReloading = true;

        GunReloadEventArgs eventArgs = new()
        {
            PlayerID = ownerID,
            Item = this,
            ReloadTime = stats.ReloadTime
        };
        EventManager.EventHandler.OnGunReload(eventArgs);

        yield return new WaitForSecondsRealtime(stats.ReloadTime);

        AudioManager.Instance.PlaySFX(stats.ReloadSFXs[UnityEngine.Random.Range(0, stats.ReloadSFXs.Length)], UnityEngine.Random.Range(0.7f, 1.1f));

        int previousValue = currentAmmo;
        currentAmmo = stats.MagazineSize;
        isReloading = false;

        AmmoChangedEventArgs ammoChangedEventArgs = new()
        {
            OwnerID = ownerID,
            Item = this,
            PreviousValue = previousValue,
            CurrentValue = currentAmmo
        };
        EventManager.EventHandler.OnAmmoChanged(ammoChangedEventArgs);
    }

    public void SpawnBullets(Quaternion rotation)
    {
        float thetaInDeg = transform.eulerAngles.z;
        float thetaInRadian = Mathf.Deg2Rad * thetaInDeg;
        float xOffset = transform.position.x + stats.BulletSpawnOffset * Mathf.Cos(thetaInRadian);
        float yOffset = transform.position.y + stats.BulletSpawnOffset * Mathf.Sin(thetaInRadian);

        NetworkObject bulletNetworkObject = NetworkObjectPool.Singleton.GetNetworkObject(stats.BulletGO, new Vector3(xOffset, yOffset, 0), Quaternion.identity);
        ProjectileMovement projectileMovement = bulletNetworkObject.GetComponent<ProjectileMovement>();
        projectileMovement.IntializeInfo
        (
            stats.BulletGO,
            rotation,
            stats.Damage,
            stats.Penetration,
            stats.Accuracy,
            stats.BulletVelocity,
            stats.Range
        );
        bulletNetworkObject.Spawn();
    }

    public void OnPickUp(InventoryHandler playerInventory)
    {
        isPickedUp = true;
        owner = playerInventory.Owner;
        ownerID = playerInventory.OwnerID;
        OnPickUpClientRpc(RpcTarget.Single(ownerID, RpcTargetUse.Temp));

        EventManager.EventHandler.OnItemLeftClickPressedEvent += OnLeftClickPressed;
        EventManager.EventHandler.OnItemLeftClickReleasesedEvent += OnLeftClickReleased;
        EventManager.EventHandler.OnitemReloadEvent += OnReloadPressed;
        EventManager.EventHandler.OnItemSwappedEvent += OnSwapIn;
    }
    [Rpc(SendTo.SpecifiedInParams)]
    void OnPickUpClientRpc(RpcParams rpcParams)
    {
        isPickedUp = true;
        NetworkManager.NetworkTickSystem.Tick += OnNetworkTick;
    }

    public void OnDrop(InventoryHandler playerInventory)
    {
        isPickedUp = false;
        OnDropClientRpc(RpcTarget.Single(ownerID, RpcTargetUse.Temp));

        owner = null;
        ownerID = 0;

        transform.rotation = Quaternion.identity;
        EventManager.EventHandler.OnItemLeftClickPressedEvent -= OnLeftClickPressed;
        EventManager.EventHandler.OnItemLeftClickReleasesedEvent -= OnLeftClickReleased;
        EventManager.EventHandler.OnitemReloadEvent -= OnReloadPressed;
        EventManager.EventHandler.OnItemSwappedEvent -= OnSwapIn;
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void OnDropClientRpc(RpcParams rpcParams)
    {
        isPickedUp = false;
        NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
    }

    void OnNetworkTick()
    {

        // Count off until we reach the ControlTicksPerUpdate which will roll the TickCounter to zero
        // and that signals we update the user's mouse information
        tickCounter = (tickCounter + 1) % TICK_PER_UPDATE;
        if (tickCounter == 0)
        {
            UpdateMouseToPlayerVector();
        }
    }

    void OnLeftClickPressed(object sender, ItemLeftClickPressedEventArgs e)
    {
        if (e.PlayerID != ownerID)
            return;
        isShooting = true;
    }
    void OnLeftClickReleased(object sender, ItemLeftClickReleasedEventArgs e)
    {
        if (e.PlayerID != ownerID)
            return;
        isShooting = false;
    }

    void OnReloadPressed(object sender, ItemReloadEventArgs e)
    {
        if (e.PlayerID != ownerID)
            return;
        Reload();
    }

    void OnSwapIn(object sender, ItemSwappedEventArgs e)
    {
        if (e.PlayerID != ownerID)
            return;
        Cursor.SetCursor(stats.Cursor, Vector2.zero, CursorMode.Auto);
    }

    public void LoadData(ItemData data)
    {
        currentAmmo = data.IntMap["CurrentAmmo"];
    }

    public void SaveData(ref ItemData data)
    {
        data.IntMap["CurrentAmmo"] = currentAmmo;
    }
}