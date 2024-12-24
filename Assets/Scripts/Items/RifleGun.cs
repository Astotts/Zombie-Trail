using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RifleGun : NetworkBehaviour, IItem, IOnLeftClickEffectItem, IOnSwapInEffectItem, IOnReloadEffectItem
{
    public event EventHandler<int> OnAmmoChangeEvent;
    public event EventHandler<float> OnReloadEvent;
    private int currentAmmo;
    public string ItemName => stats.GunName;
    public Sprite Icon => stats.Icon;
    public Sprite WeaponSprite => stats.WeaponSprite;
    public int CurrentUses => currentAmmo;
    public int Capacity => stats.Capacity;
    public NetworkObject WeaponNetworkObject => networkObject;

    [SerializeField] private GunStats stats;
    [SerializeField] NetworkObject networkObject;
    [SerializeField] SpriteRenderer weaponSpriteRenderer;

    private bool isReloading;
    private bool isOnFireRateCooldown;

    void OnValidate()
    {
        networkObject = GetComponent<NetworkObject>();
    }

    void Start()
    {
        currentAmmo = UnityEngine.Random.Range(0, stats.MagazineSize);
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

        // get the mouse position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // current position to mouse position
        Vector2 directionOfAttack = (mousePos - transform.position).normalized;

        // create & shoot the projectile 
        SpawnBulletsServerRpc(directionOfAttack);
        StartCoroutine(FireRateCooldown());
        currentAmmo -= 1;
        AudioManager.Instance.PlaySFX(stats.GunShotSFXs[UnityEngine.Random.Range(0, stats.GunShotSFXs.Length)], UnityEngine.Random.Range(0.7f, 1.1f));
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
    public void SpawnBulletsServerRpc(Vector2 direction)
    {
        NetworkObject bulletNetworkObject = NetworkObjectPool.Singleton.GetNetworkObject(stats.BulletGO, transform.position, Quaternion.identity);
        ProjectileMovement projectileMovement = bulletNetworkObject.GetComponent<ProjectileMovement>();
        projectileMovement.IntializeInfo
        (
            stats.BulletGO,
            direction,
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
}