using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using static EventManager;

public class Shotgun : NetworkBehaviour, IItem, IDisplayableWeapon
{
    private static readonly float GLOBAL_ROTATE_SPEED = 10000;           // Only change when you want to change rotation speed for other gun too


    public Guid UniqueID => guid;                                       // Guid for every prefab
    public EItemType ItemType => EItemType.Shotgun;                       // Item type for prefab
    public string WeaponName => stats.GunName;                             // Variable for WeaponPanelUI to read
    public Sprite Icon => stats.Icon;                                   // Variable for WeaponPanelUI and HotbarUI to read
    public int CurrentAmmo => currentAmmo.Value;                        // Variable for WeaponPanelUI to read
    public Sprite AmmoIcon => stats.AmmoIcon;                           // Variable for WeaponPanelUI to read
    public Sprite EmptyAmmoIcon => stats.EmptyAmmoIcon;                 // Variable for WeaponPanelUI to read
    public int MagazineSize => stats.MagazineSize;                      // Variable for WeaponPanelUI to read
    public int CurrentMagazine => currentAmmoHolding.Value;                // Variable for WeaponPanelUI to read
    public int MaxMagazine => stats.MaxAmmoToHold;                              // Variable for WeaponPanelUI to read
    public NetworkObject WeaponNetworkObject => networkObject;          // Variable for WeaponPanelUI to read


    [SerializeField] AvailableItemsSO availableItemsSO;                 // Script to save and load item to json
    [SerializeField] private ShotgunGunStats stats;                       // Stats for guns (this script would be used for Shotgun GameObjects)
    [SerializeField] NetworkObject networkObject;                       // Just for external variable up there (WeaponNetworkObject)
    [SerializeField] SpriteRenderer weaponSpriteRenderer;               // Sprite render so we can flip it

    // Owner's mouse to player
    private readonly NetworkVariable<int> currentAmmo = new();          // Current Ammo before reload
    private readonly NetworkVariable<int> currentAmmoHolding = new();   // Ammo left for reloading
    private readonly NetworkVariable<int> currentLayer = new();         // For syncing layer between clients (Rpc doesn't work for late joining clients)
    private readonly NetworkVariable<bool> isActive = new(true);        // For syncing gameobject activation on player pick ups

    private Rigidbody2D ownerRigid2D;                                    // Owner Rigid2D for recoils
    private Guid guid;
    private ulong ownerID;                                              // ClientId of owner in ulong
    private bool isReloading;                                           // Inner variable so we know when the gun is reloading (can't shoot)
    private bool isPickedUp;                                            // Inner variable so that gun can rotate
    private bool IsOnFireRateCooldown => fireRateCooldown > 0;         // Inner variable so we know when to shoot
    private float fireRateCooldown;

    private Coroutine reloadingCoroutine;

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
        currentAmmoHolding.Value = UnityEngine.Random.Range(0, stats.MaxAmmoToHold);
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
        EventManager.EventHandler.OnItemReloadPressedEvent -= OnReloadPressed;
        EventManager.EventHandler.OnItemSwapPressedEvent -= OnItemSwap;
        base.OnNetworkDespawn();
    }

    private void OnItemSwap(object sender, ItemSwapPressedEventArgs e)
    {
        if (e.PlayerID != ownerID)
            return;

        InterruptReloading();

        isActive.Value = e.CurrentItem != null && e.CurrentItem.UniqueID == this.guid;
    }

    void Update()
    {
        if (!isPickedUp)
            return;

        if (IsServer && IsOnFireRateCooldown)
            fireRateCooldown -= Time.deltaTime;

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
        if (IsOnFireRateCooldown)
            return;

        if (currentAmmo.Value == 0)
        {
            Reload();
            return;
        }

        InterruptReloading();

        // create & shoot the projectile 
        SpawnBullets(transform.rotation);
        fireRateCooldown = stats.FireRate;
        currentAmmo.Value -= 1;
        AudioManager.Instance.PlaySFX(stats.GunShotSFXs[UnityEngine.Random.Range(0, stats.GunShotSFXs.Length)], UnityEngine.Random.Range(0.7f, 1.1f));

        HandleRecoil();
    }

    private void Reload()
    {
        if (isReloading || currentAmmoHolding.Value < 1 || currentAmmo.Value == stats.MagazineSize)
            return;
        AudioManager.Instance.PlaySFX(stats.ReloadSFXs[UnityEngine.Random.Range(0, stats.ReloadSFXs.Length)], UnityEngine.Random.Range(0.7f, 1.1f));
        reloadingCoroutine = StartCoroutine(Reloading());
    }

    void HandleRecoil()
    {
        float thetaInDeg = transform.eulerAngles.z;
        float thetaInRadian = Mathf.Deg2Rad * thetaInDeg;
        float xOffset = transform.position.x + stats.BulletSpawnOffset * Mathf.Cos(thetaInRadian);
        float yOffset = transform.position.y + stats.BulletSpawnOffset * Mathf.Sin(thetaInRadian);
        Vector2 bulletOffset = new(xOffset, yOffset);
        Vector2 originPos = transform.position;

        Vector2 fromBulletToPlayer = originPos - bulletOffset;
        Vector2 recoilVector = fromBulletToPlayer.normalized * stats.Recoil;
        ApplyRecoilClientRpc(recoilVector, RpcTarget.Single(ownerID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void ApplyRecoilClientRpc(Vector2 vectorForce, RpcParams rpcParams)
    {
        ownerRigid2D.AddForce(vectorForce, ForceMode2D.Impulse);
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

        currentAmmo.Value++;
        currentAmmoHolding.Value--;
        isReloading = false;

        GunReloadedEventArgs reloadedEventArgs = new()
        {
            PlayerID = ownerID,
            Item = this,
            CurrentMagazine = currentAmmoHolding.Value,
            MaxMagazine = stats.MaxAmmoToHold
        };
        EventManager.EventHandler.OnGunReloaded(reloadedEventArgs);

        if (currentAmmo.Value < stats.MagazineSize && currentAmmoHolding.Value > 0)
            reloadingCoroutine = StartCoroutine(Reloading());
    }

    public void SpawnBullets(Quaternion rotation)
    {
        float thetaInDeg = transform.eulerAngles.z;
        float thetaInRadian = Mathf.Deg2Rad * thetaInDeg;
        float xOffset = transform.position.x + stats.BulletSpawnOffset * Mathf.Cos(thetaInRadian);
        float yOffset = transform.position.y + stats.BulletSpawnOffset * Mathf.Sin(thetaInRadian);
        for (int i = 0; i < stats.BulletsPerShot; i++)
        {
            NetworkObject bulletNetworkObject = NetworkObjectPool.Singleton.GetNetworkObject(stats.BulletGO, new Vector3(xOffset, yOffset, 0), Quaternion.identity);
            ProjectileMovement projectileMovement = bulletNetworkObject.GetComponent<ProjectileMovement>();
            projectileMovement.IntializeInfo
            (
                stats.BulletGO,
                rotation,
                stats.DamagePerBullet,
                stats.Penetration,
                stats.Focus,
                stats.BulletVelocity,
                stats.Range
            );
            bulletNetworkObject.Spawn();
        }
    }

    void OnPickUp(NetworkObject owner, ulong ownerID, bool isActive)
    {
        InterruptReloading();
        isPickedUp = true;
        this.ownerID = ownerID;
        this.isActive.Value = isActive;
        this.ownerRigid2D = owner.GetComponent<Rigidbody2D>();
        currentLayer.Value = LayerMask.NameToLayer("IgnorePickUpRaycast");
        OnPickUpClientRpc(owner, RpcTarget.Single(ownerID, RpcTargetUse.Temp));

        EventManager.EventHandler.OnItemLeftClickPressedEvent += OnLeftClickPressed;
        EventManager.EventHandler.OnItemReloadPressedEvent += OnReloadPressed;
        EventManager.EventHandler.OnItemSwapPressedEvent += OnItemSwap;
    }
    [Rpc(SendTo.SpecifiedInParams)]
    void OnPickUpClientRpc(NetworkObjectReference networkObjectReference, RpcParams rpcParams)
    {
        if (!networkObjectReference.TryGet(out NetworkObject networkObject))
            return;
        isPickedUp = true;
        ownerRigid2D = networkObject.GetComponent<Rigidbody2D>();
    }

    void OnDrop()
    {
        InterruptReloading();
        isPickedUp = false;
        ownerID = 0;
        ownerRigid2D = null;
        isActive.Value = true;
        currentLayer.Value = LayerMask.NameToLayer("PickUpRaycast");
        OnDropClientRpc(RpcTarget.Single(ownerID, RpcTargetUse.Temp));

        transform.rotation = Quaternion.identity;
        EventManager.EventHandler.OnItemLeftClickPressedEvent -= OnLeftClickPressed;
        EventManager.EventHandler.OnItemReloadPressedEvent -= OnReloadPressed;
        EventManager.EventHandler.OnItemSwapPressedEvent -= OnItemSwap;
    }

    void InterruptReloading()
    {
        if (isReloading)
        {
            StopCoroutine(reloadingCoroutine);
            isReloading = false;
            GunReloadInterruptedEventArgs eventArgs = new()
            {
                PlayerID = ownerID,
                Item = this
            };
            EventManager.EventHandler.GunReloadInterupted(eventArgs);
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void OnDropClientRpc(RpcParams rpcParams)
    {
        isPickedUp = false;
        ownerRigid2D = null;
    }

    void OnLeftClickPressed(object sender, ItemLeftClickPressedEventArgs e)
    {
        if (e.PlayerID != ownerID || e.Item.UniqueID != this.guid)
            return;
        HandleShooting();
    }

    void OnReloadPressed(object sender, ItemReloadPressedEventArgs e)
    {
        if (e.PlayerID != ownerID || e.Item.UniqueID != this.guid)
            return;
        Reload();
    }

    public void LoadData(ItemData data, ulong ownerID, int currentSlot, int loadedSlot)
    {
        if (data.Stats is not ShotgunStats ShotgunStats)
            return;

        guid = new Guid(ShotgunStats.UniqueID);
        currentAmmo.Value = ShotgunStats.CurrentAmmo;
        currentAmmoHolding.Value = ShotgunStats.CurrentAmmoHolding;
        stats = (ShotgunGunStats)availableItemsSO.GetItemStat(EItemType.Shotgun, ShotgunStats.GunStatsID);
        NetworkObject ownerNetworkObject = NetworkManager.Singleton.ConnectedClients[ownerID].PlayerObject;
        OnPickUp(ownerNetworkObject, ownerID, currentSlot == loadedSlot);
    }

    public void SaveData(ref ItemData data)
    {
        ShotgunStats ShotgunStats = new()
        {
            UniqueID = guid.ToString(),
            CurrentAmmo = currentAmmo.Value,
            CurrentAmmoHolding = currentAmmoHolding.Value,
            GunStatsID = availableItemsSO.GetItemStatIndex(EItemType.Shotgun, stats)
        };
        data.Stats = ShotgunStats;
    }

    public class ShotgunStats : IItemStats
    {
        public string UniqueID { get; set; }
        public int CurrentAmmo { get; set; }
        public int CurrentAmmoHolding { get; set; }
        public int GunStatsID { get; set; }
    }
}
