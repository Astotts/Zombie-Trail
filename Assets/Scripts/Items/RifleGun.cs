using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class RifleGun : NetworkBehaviour, IItem, IOnLeftClickEffectItem, IOnSwapInEffectItem, IOnReloadEffectItem, IOnPickupEffectItem
{
    [SerializeField] private float GLOBAL_ROTATE_SPEED = 5000;         // Only change when you want to change rotation speed for other gun too

    public event EventHandler<int> OnAmmoChangeEvent;               // Event for UI (mostly)
    public event EventHandler<float> OnReloadEvent;                 // Event for UI (mostly)
    private int currentAmmo;                                        // Current Ammo before reload
    public string ItemName => stats.GunName;                        // Variables for Inventory and UIs to read
    public Sprite Icon => stats.Icon;                               // Variables for Inventory and UIs to read
    public Sprite WeaponSprite => stats.WeaponSprite;               // Variables for Inventory and UIs to read
    public int CurrentUses => currentAmmo;                          // Variables for Inventory and UIs to read
    public int Capacity => stats.Capacity;                          // Variables for Inventory and UIs to read
    public NetworkObject WeaponNetworkObject => networkObject;      // Variables for Inventory and UIs to read

    [SerializeField] private GunStats stats;                        // Stats for guns (this script would be use for rifles GameObject)
    [SerializeField] NetworkObject networkObject;                   // Just for external variable up there (WeaponNetworkObject)
    [SerializeField] SpriteRenderer weaponSpriteRenderer;           // Sprite render so we can flip it

    private bool isReloading;                                       // Inner variable so we know when the gun is reloading (can't shoot)
    private bool isOnFireRateCooldown;                              // Inner variable so we know when to shoot
    private bool isPickedUp;                                        // Inner variable so that gun can rotate

    void OnValidate()
    {
        networkObject = GetComponent<NetworkObject>();
    }

    void Start()
    {
        currentAmmo = UnityEngine.Random.Range(0, stats.MagazineSize);
    }

    void Update()
    {
        if (!isPickedUp)
            return;


        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Finding the closest player? Look like a working as a magnet
        // target = targetFinder.GetClosest();
        // moveDirection = (Vector2)target.position - (Vector2)characterPos.position;
        // //Debug.DrawRay(transform.position, targetWorldPos, Color.red, 0.01f);

        // vector from this object towards the target location
        Vector3 vectorToTarget = mouseWorldPos - (Vector2)transform.position;
        // rotate that vector by 90 degrees around the Z axis
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * vectorToTarget;

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

    public void OnLeftClick(NetworkObject player)
    {
        if (isReloading || isOnFireRateCooldown)
            return;

        if (currentAmmo == 0)
        {
            OnReload(player);
            return;
        }

        // create & shoot the projectile 
        SpawnBulletsServerRpc(transform.rotation);
        StartCoroutine(FireRateCooldown());
        currentAmmo -= 1;
        AudioManager.Instance.PlaySFX(stats.GunShotSFXs[UnityEngine.Random.Range(0, stats.GunShotSFXs.Length)], UnityEngine.Random.Range(0.7f, 1.1f));

        HandleRecoil();
    }

    void HandleRecoil()
    {
        Vector2 currentMousePos = Input.mousePosition;
        Mouse.current.WarpCursorPosition(new Vector2(currentMousePos.x, currentMousePos.y + stats.Recoil));
    }

    IEnumerator FireRateCooldown()
    {
        isOnFireRateCooldown = true;
        yield return new WaitForSecondsRealtime(stats.FireRate);
        isOnFireRateCooldown = false;
    }

    public void OnReload(NetworkObject networkObject)
    {
        if (!isReloading)
        {
            isReloading = true;
            AudioManager.Instance.PlaySFX(stats.ReloadSFXs[UnityEngine.Random.Range(0, stats.ReloadSFXs.Length)], UnityEngine.Random.Range(0.7f, 1.1f));
            StartCoroutine(Reloading());
        }
    }

    IEnumerator Reloading()
    {
        OnReloadEvent?.Invoke(this, stats.ReloadTime);
        yield return new WaitForSecondsRealtime(stats.ReloadTime);
        AudioManager.Instance.PlaySFX(stats.ReloadSFXs[UnityEngine.Random.Range(0, stats.ReloadSFXs.Length)], UnityEngine.Random.Range(0.7f, 1.1f));
        currentAmmo = stats.MagazineSize;
        OnAmmoChange(currentAmmo);
        isReloading = false;
    }

    [Rpc(SendTo.Server)]
    public void SpawnBulletsServerRpc(Quaternion rotation)
    {
        NetworkObject bulletNetworkObject = NetworkObjectPool.Singleton.GetNetworkObject(stats.BulletGO, transform.position, Quaternion.identity);
        ProjectileMovement projectileMovement = bulletNetworkObject.GetComponent<ProjectileMovement>();
        projectileMovement.IntializeInfo
        (
            stats.BulletGO,
            rotation,
            stats.Damage,
            stats.Accuracy,
            stats.Penetration,
            stats.BulletVelocity,
            stats.Range
        );
        bulletNetworkObject.Spawn();
    }

    public void OnSwapIn(NetworkObject player)
    {
        Cursor.SetCursor(stats.Cursor, Vector2.zero, CursorMode.Auto);
    }

    void OnAmmoChange(int currentAmmo)
    {
        OnAmmoChangeEvent?.Invoke(this, currentAmmo);
    }

    public void OnPickUp(NetworkObject player)
    {
        isPickedUp = true;
    }
}