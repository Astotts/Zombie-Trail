using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPanelUI : MonoBehaviour
{
    [SerializeField] TMP_Text weaponNameTMP;
    [SerializeField] Image weaponImage;
    [SerializeField] RectTransform ammoAreaRectTransform;
    [SerializeField] RectTransform ammoImageRectTransform;
    [SerializeField] RectTransform emptyAmmoImageRectTransform;
    [SerializeField] Image ammoImage;
    [SerializeField] Image emptyAmmoImage;

    RifleGun gun;
    float ammoIconWidth;
    float emptyAmmoIconWidth;

    void OnEnable()
    {
        InventoryManager.OnItemSwapEvent += OnItemSwap;
    }

    void OnDisable()
    {
        InventoryManager.OnItemSwapEvent -= OnItemSwap;
        if (gun != null)
            gun.OnAmmoChangeEvent -= OnGunShot;
    }

    private void OnItemSwap(object sender, InventoryManager.ItemSwappedEventArgs e)
    {
        if (e.CurrentItem is not RifleGun gun)
        {
            DisableUI();
            this.gun.OnAmmoChangeEvent -= OnGunShot;
            this.gun = null;
            return;
        }
        EnableUI();

        this.gun = gun;
        gun.OnAmmoChangeEvent += OnGunShot;

        weaponNameTMP.text = gun.GunName;
        weaponImage.sprite = gun.Icon;

        SetEmptyAmmoIcon(gun.EmptyAmmoIcon, gun.MagazineSize);
        SetAmmoIcon(gun.AmmoIcon, gun.MagazineSize);
        SetCurrentAmmo(gun.CurrentAmmo);
    }

    void DisableUI()
    {
        weaponNameTMP.gameObject.SetActive(false);
        weaponImage.gameObject.SetActive(false);
        ammoAreaRectTransform.gameObject.SetActive(false);
    }
    void EnableUI()
    {
        weaponNameTMP.gameObject.SetActive(true);
        weaponImage.gameObject.SetActive(true);
        ammoAreaRectTransform.gameObject.SetActive(true);
    }

    void SetAmmoIcon(Sprite ammoIcon, int maxAmount)
    {
        ammoImage.sprite = ammoIcon;
        ammoImage.SetNativeSize();
        ammoIconWidth = ammoImageRectTransform.sizeDelta.x;
        float rectWidth = maxAmount * ammoIconWidth;
        float ammoAreaWidth = ammoAreaRectTransform.sizeDelta.x;
        if (rectWidth > ammoAreaWidth)
        {
            float scale = ammoAreaWidth / rectWidth;
            ammoImageRectTransform.localScale = new Vector3(scale, scale, scale);
        }
    }

    void SetCurrentAmmo(int currentAmount)
    {
        ammoImageRectTransform.sizeDelta = new(currentAmount * ammoIconWidth, ammoImageRectTransform.sizeDelta.y);
    }

    void SetEmptyAmmoIcon(Sprite emptyAmmoIcon, int maxAmount)
    {
        emptyAmmoImage.sprite = emptyAmmoIcon;
        emptyAmmoImage.SetNativeSize();
        emptyAmmoIconWidth = emptyAmmoImageRectTransform.sizeDelta.x;
        float rectWidth = maxAmount * emptyAmmoIconWidth;
        float rectHeight = emptyAmmoImageRectTransform.sizeDelta.y;
        emptyAmmoImageRectTransform.sizeDelta = new(rectWidth, rectHeight);
        float ammoAreaWidth = ammoAreaRectTransform.sizeDelta.x;
        if (rectWidth > ammoAreaWidth)
        {
            float scale = ammoAreaWidth / rectWidth;
            emptyAmmoImageRectTransform.localScale = new Vector3(scale, scale, scale);
        }
    }

    void OnGunShot(object sender, int currentAmmo)
    {
        SetCurrentAmmo(currentAmmo);
    }
}