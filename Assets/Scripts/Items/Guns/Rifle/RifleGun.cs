using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using static EventManager;

public class RifleGun : NetworkBehaviour, IItem, IDisplayableWeapon
{
    private static readonly float GLOBAL_RECOIL_RESISTANCE = 100;       // Only change when you want to change rotation speed for other gun too
    private static readonly float GLOBAL_ROTATE_SPEED = 1000;           // Only change when you want to change rotation speed for other gun too


    public Guid UniqueID => guid;                                       // Guid for every prefab
    public EItemType ItemType => EItemType.Rifle;                       // Item type for prefab
    public string WeaponName => stats.name;                             // Variable for WeaponPanelUI to read
    public Sprite Icon => stats.Icon;                                   // Variable for WeaponPanelUI and HotbarUI to read
    public int CurrentAmmo => currentAmmo.Value;                        // Variable for WeaponPanelUI to read
    public Sprite AmmoIcon => stats.AmmoIcon;                           // Variable for WeaponPanelUI to read
    public Sprite EmptyAmmoIcon => stats.EmptyAmmoIcon;                 // Variable for WeaponPanelUI to read
    public int MagazineSize => stats.MagazineSize;                      // Variable for WeaponPanelUI to read
    public int CurrentMagazine => currentMagazine.Value;                // Variable for WeaponPanelUI to read
    public int MaxMagazine => stats.Capacity;                              // Variable for WeaponPanelUI to read
    public NetworkObject WeaponNetworkObject => networkObject;          // Variable for WeaponPanelUI to read


    [SerializeField] AvailableItemsSO availableItemsSO;                 // Script to save and load item to json
    [SerializeField] private RifleGunStats stats;                       // Stats for guns (this script would be use for rifles GameObjects)
    [SerializeField] NetworkObject networkObject;                       // Just for external variable up there (WeaponNetworkObject)
    [SerializeField] SpriteRenderer weaponSpriteRenderer;               // Sprite render so we can flip it

    // Owner's mouse to player
    private readonly NetworkVariable<int> currentAmmo = new();          // Current Ammo before reload
    private readonly NetworkVariable<int> currentMagazine = new();      // Magazine left for reloading
    private readonly NetworkVariable<int> currentLayer = new();         // For syncing layer between clients (Rpc doesn't work for late joining clients)
    private readonly NetworkVariable<bool> isActive = new(true);        // For syncing gameobject activation on player pick ups

    private NetworkObject owner;                                        // Owner of the gun
    private Guid guid;
    private ulong ownerID;                                              // ClientId of owner in ulong
    private bool isReloading;                                           // Inner variable so we know when the gun is reloading (can't shoot)
    private bool isOnFireRateCooldown;                                  // Inner variable so we know when to shoot
    private bool isPickedUp;                                            // Inner variable so that gun can rotate
    private bool isShooting;                                            // Inner variable so we know left click is down

    void OnValidate()
    {
        networkObject = GetComponent<NetworkObject>();
        if (weaponSpriteRenderer != null)
            weaponSpriteRenderer.sprite = stats.WeaponSprite;
    }

    public override void OnNetworkSpawn()
    {
        gameObject.layer = currentLayer.Value;
        gameObject.SetActive(isActive.Value);

        currentLayer.OnValueChanged += OnLayerChange;
        isActive.OnValueChanged += OnActiveChange;
        if (!IsServer)
            return;

        // Server initialize these values on networkObject spawned;
        guid = Guid.NewGuid();
        currentLayer.Value = LayerMask.NameToLayer("PickUpRaycast");
        currentAmmo.Value = UnityEngine.Random.Range(0, stats.MagazineSize);
        currentMagazine.Value = UnityEngine.Random.Range(0, stats.Capacity);
        currentAmmo.OnValueChanged += OnAmmoChange;

        EventManager.EventHandler.OnItemPickUpPressedEvent += OnItemPickedUp;
        EventManager.EventHandler.OnItemDropPressedEvent += OnItemDropped;
    }

    private void OnItemPickedUp(object sender, ItemPickUpPressedEventArgs e)
    {
        if (e.Item == null || e.Item.UniqueID != this.guid)
            return;

        NetworkObject ownerNetworkObject = NetworkManager.Singleton.ConnectedClients[e.PlayerID].PlayerObject;
        ulong ownerID = e.PlayerID;
        bool isItemActive = e.CurrentSlot == e.PickedUpSlot;

        OnPickUp(ownerNetworkObject, ownerID, isItemActive);
    }

    private void OnItemDropped(object sender, ItemDropPressedEventArgs e)
    {
        if (e.Item == null || e.Item.UniqueID != this.guid)
            return;

        OnDrop();
    }

    private void OnActiveChange(bool previousValue, bool newValue)
    {
        gameObject.SetActive(newValue);
    }

    private void OnLayerChange(int previousValue, int newValue)
    {
        gameObject.layer = newValue;
    }

    private void OnAmmoChange(int previousValue, int newValue)
    {
        AmmoChangedEventArgs ammoChangedEventArgs = new()
        {
            OwnerID = ownerID,
            Item = this,
            PreviousValue = previousValue,
            CurrentValue = newValue
        };
        EventManager.EventHandler.OnAmmoChanged(ammoChangedEventArgs);
    }

    public override void OnNetworkDespawn()
    {
        if (!isPickedUp)
            return;

        EventManager.EventHandler.OnItemLeftClickPressedEvent -= OnLeftClickPressed;
        EventManager.EventHandler.OnItemLeftClickReleasesedEvent -= OnLeftClickReleased;
        EventManager.EventHandler.OnItemReloadPressedEvent -= OnReloadPressed;
        EventManager.EventHandler.OnItemSwapPressedEvent -= OnItemSwap;
        base.OnNetworkDespawn();
    }

    private void OnItemSwap(object sender, ItemSwapPressedEventArgs e)
    {
        if (e.PlayerID != ownerID)
            return;

        isActive.Value = e.CurrentItem != null && e.CurrentItem.UniqueID == this.guid;
    }

    void Update()
    {
        if (!isPickedUp)
            return;

        if (IsServer)
            HandleShooting();

        if (IsOwner)
            HandleWeaponRotation();
    }

    void HandleWeaponRotation()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Finding the closest player? Look like a working as a magnet
        // target = targetFinder.GetClosest();
        // moveDirection = (Vector2)target.position - (Vector2)characterPos.position;
        // //Debug.DrawRay(transform.position, targetWorldPos, Color.red, 0.01f);

        // vector from this object towards the target location
        Vector2 mouseToPlayerVector = mouseWorldPos - (Vector2)transform.position;

        // rotate that vector by 90 degrees around the Z axis
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * mouseToPlayerVector;

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
    }

    public void HandleShooting()
    {
        if (!isShooting || isReloading || isOnFireRateCooldown)
            return;

        if (currentAmmo.Value == 0)
        {
            Reload();
            return;
        }

        // create & shoot the projectile 
        SpawnBullets(transform.rotation);
        StartCoroutine(FireRateCooldown());
        currentAmmo.Value -= 1;
        AudioManager.Instance.PlaySFX(stats.GunShotSFXs[UnityEngine.Random.Range(0, stats.GunShotSFXs.Length)], UnityEngine.Random.Range(0.7f, 1.1f));

        HandleRecoil();
    }

    private void Reload()
    {
        if (isReloading || currentMagazine.Value < 1 || currentAmmo.Value == stats.MagazineSize)
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
        ApplyRecoilClientRpc(recoilVector, RpcTarget.Single(ownerID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void ApplyRecoilClientRpc(Vector2 vectorForce, RpcParams rpcParams)
    {
        owner.transform.position = owner.transform.position + (Vector3)vectorForce;
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

        currentAmmo.Value = stats.MagazineSize;
        currentMagazine.Value--;
        isReloading = false;

        GunReloadedEventArgs reloadedEventArgs = new()
        {
            PlayerID = ownerID,
            Item = this,
            CurrentMagazine = currentMagazine.Value,
            MaxMagazine = stats.Capacity
        };
        EventManager.EventHandler.OnGunReloaded(reloadedEventArgs);
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

    public void OnPickUp(NetworkObject owner, ulong ownerID, bool isActive)
    {
        isPickedUp = true;
        this.owner = owner;
        this.ownerID = ownerID;
        this.isActive.Value = isActive;
        currentLayer.Value = LayerMask.NameToLayer("IgnorePickUpRaycast");
        OnPickUpClientRpc(owner, RpcTarget.Single(ownerID, RpcTargetUse.Temp));

        EventManager.EventHandler.OnItemLeftClickPressedEvent += OnLeftClickPressed;
        EventManager.EventHandler.OnItemLeftClickReleasesedEvent += OnLeftClickReleased;
        EventManager.EventHandler.OnItemReloadPressedEvent += OnReloadPressed;
        EventManager.EventHandler.OnItemSwapPressedEvent += OnItemSwap;
    }
    [Rpc(SendTo.SpecifiedInParams)]
    void OnPickUpClientRpc(NetworkObjectReference networkObjectReference, RpcParams rpcParams)
    {
        if (!networkObjectReference.TryGet(out NetworkObject networkObject))
            return;
        isPickedUp = true;
        owner = networkObject;
    }

    public void OnDrop()
    {
        isPickedUp = false;
        owner = null;
        ownerID = 0;
        isActive.Value = true;
        currentLayer.Value = LayerMask.NameToLayer("PickUpRaycast");
        OnDropClientRpc(RpcTarget.Single(ownerID, RpcTargetUse.Temp));

        transform.rotation = Quaternion.identity;
        EventManager.EventHandler.OnItemLeftClickPressedEvent -= OnLeftClickPressed;
        EventManager.EventHandler.OnItemLeftClickReleasesedEvent -= OnLeftClickReleased;
        EventManager.EventHandler.OnItemReloadPressedEvent -= OnReloadPressed;
        EventManager.EventHandler.OnItemSwapPressedEvent -= OnItemSwap;
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void OnDropClientRpc(RpcParams rpcParams)
    {
        isPickedUp = false;
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

    void OnReloadPressed(object sender, ItemReloadPressedEventArgs e)
    {
        if (e.PlayerID != ownerID)
            return;
        Reload();
    }

    public void LoadData(ItemData data, ulong ownerID, int currentSlot, int loadedSlot)
    {
        if (data.Stats is not RifleStats rifleStats)
            return;

        guid = new Guid(rifleStats.UniqueID);
        currentAmmo.Value = rifleStats.CurrentAmmo;
        currentMagazine.Value = rifleStats.CurrentMagazine;
        stats = (RifleGunStats)availableItemsSO.GetItemStat(EItemType.Rifle, rifleStats.GunStatsID);
        NetworkObject ownerNetworkObject = NetworkManager.Singleton.ConnectedClients[ownerID].PlayerObject;
        OnPickUp(ownerNetworkObject, ownerID, currentSlot == loadedSlot);
    }

    public void SaveData(ref ItemData data)
    {
        RifleStats rifleStats = new()
        {
            UniqueID = guid.ToString(),
            CurrentAmmo = currentAmmo.Value,
            CurrentMagazine = currentMagazine.Value,
            GunStatsID = availableItemsSO.GetItemStatIndex(EItemType.Rifle, stats)
        };
        data.Stats = rifleStats;
    }

    public class RifleStats : IItemStats
    {
        public string UniqueID { get; set; }
        public int CurrentAmmo { get; set; }
        public int CurrentMagazine { get; set; }
        public int GunStatsID { get; set; }
    }
}