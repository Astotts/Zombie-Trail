using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RifleGun : NetworkBehaviour, IItem, IOnLeftClickEffect, IOnRightClickEffect, IOnSwapInEffect
{
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
    [SerializeField] Image[] uiImage;
    [SerializeField] GameObject reloadFill;

    private float elapsed;
    private bool isReloading;

    void OnValidate()
    {
        networkObject = GetComponent<NetworkObject>();
    }

    public void OnLeftClick(GameObject player)
    {
        if (currentAmmo == 0)
        {
            Reload();
        }
        else if (0 >= elapsed)
        {
            elapsed = stats.Recoil;
            // get the mouse position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // current position to mouse position
            Vector2 directionOfAttack = (mousePos - transform.position).normalized;

            // create & shoot the projectile 
            SpawnBulletsServerRpc(directionOfAttack);
            currentAmmo -= 1;
            AudioManager.Instance.PlaySFX(stats.GunShotSFXs[UnityEngine.Random.Range(0, 1)], UnityEngine.Random.Range(0.7f, 1.1f));
        }
    }

    public void Reload()
    {
        if (!isReloading)
        {
            isReloading = true;
            AudioManager.Instance.PlaySFX(stats.ReloadSFXs[UnityEngine.Random.Range(0, 2)], UnityEngine.Random.Range(0.7f, 1.1f));
            StartCoroutine(Reloading());
        }
    }
    IEnumerator Reloading()
    {
        SetOpacity(1);
        reloadFill.transform.localScale = new Vector3(0, 1, 1);
        float reloadElapsed = 0;
        float reloadValue;
        while (reloadElapsed < stats.ReloadTime)
        {
            reloadElapsed += Time.deltaTime;
            reloadValue = reloadElapsed / stats.ReloadTime;
            //Debug.Log(reloadSlider.value);
            reloadFill.transform.localScale = new Vector3(reloadValue, 0, 0);
            yield return null;
        }
        currentAmmo = stats.MagazineSize;
        isReloading = false;
        AudioManager.Instance.PlaySFX(stats.ReloadSFXs[UnityEngine.Random.Range(0, 2)], UnityEngine.Random.Range(0.7f, 1.1f));
        StartCoroutine(HideReloadStatus());
    }

    IEnumerator HideReloadStatus()
    {
        float statusElapsed = 0f;
        while (statusElapsed <= stats.FadeDuration)
        {
            statusElapsed += Time.deltaTime;
            SetOpacity(Mathf.Lerp(1f, 0f, statusElapsed / stats.FadeDuration));
            yield return null;
        }
        yield break;
    }

    public void SetOpacity(float a)
    {
        //Debug.Log("Setting Opacity to " + a);
        for (int i = 0; i < uiImage.Length; i++)
        {
            uiImage[i].color = new Color(1f, 1f, 1f, a);
        }
    }

    [Rpc(SendTo.Server)]
    public void SpawnBulletsServerRpc(Vector2 direction)
    {
        GameObject newProjectile = Instantiate(stats.BulletGO, transform.position, Quaternion.identity);
        newProjectile.GetComponent<NetworkObject>().Spawn();
        newProjectile.GetComponent<ProjectileMovement>().InitiateMovement(direction, stats.BulletVelocity, stats.Damage);
    }

    public void OnRightClick(GameObject player)
    {
        throw new System.NotImplementedException();
    }

    public void OnSwapIn(GameObject player)
    {
        Cursor.SetCursor(stats.Cursor, Vector2.zero, CursorMode.Auto);
    }
}