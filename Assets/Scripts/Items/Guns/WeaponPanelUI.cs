using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static EventManager;

public class WeaponPanelUI : NetworkBehaviour
{
    [SerializeField] TMP_Text weaponNameTMP;
    [SerializeField] Image weaponImage;
    [SerializeField] RectTransform ammoAreaRectTransform;
    [SerializeField] RectTransform ammoImageRectTransform;
    [SerializeField] RectTransform emptyAmmoImageRectTransform;
    [SerializeField] Image ammoImage;
    [SerializeField] Image emptyAmmoImage;

    RifleGun currentItem;
    float ammoIconWidth;
    float emptyAmmoIconWidth;

    void Start()
    {
        if (!IsHost)
            return;
        EventHandler.OnItemSwappedEvent += OnItemSwap;
        EventHandler.OnItemPickedUpEvent += OnItemPickUp;
        EventHandler.OnItemDroppedEvent += OnItemDrop;
        EventHandler.OnGunAmmoChangedEvent += OnRifleAmmoChange;
        Debug.Log("Weapon Panel Subscribed to InventoryEvents!");
    }

    void OnDisable()
    {
        if (!IsHost)
            return;
        EventHandler.OnItemSwappedEvent -= OnItemSwap;
        EventHandler.OnItemPickedUpEvent -= OnItemPickUp;
        EventHandler.OnItemDroppedEvent -= OnItemDrop;
        EventHandler.OnGunAmmoChangedEvent -= OnRifleAmmoChange;
        Debug.Log("Weapon Panel Unsubscribed to InventoryEvents!");
    }

    // =======================
    // Server Side:
    // =====================

    private void OnItemPickUp(object sender, ItemPickedUpEventArgs e)
    {
        HandleItem(e.PlayerID, e.Item);
    }

    private void OnItemDrop(object sender, ItemDroppedEventArgs e)
    {
        HandleItem(e.PlayerID, null);
    }

    private void OnItemSwap(object sender, ItemSwappedEventArgs e)
    {
        HandleItem(e.PlayerID, e.CurrentItem);
    }

    void HandleItem(ulong playerID, IItem item)
    {
        UpdateUIClientRpc((NetworkBehaviour)item, RpcTarget.Single(playerID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void UpdateUIClientRpc(NetworkBehaviourReference clientNetworkObject, RpcParams rpcParams)
    {
        if (clientNetworkObject.TryGet(out RifleGun rifleGun))
        {
            currentItem = rifleGun;
            HandleRifleGun(rifleGun);
            EnableUI();
        }
        else
        {
            DisableUI();
        }
    }

    void HandleRifleGun(RifleGun rifleGun)
    {
        weaponImage.sprite = rifleGun.Icon;
        weaponNameTMP.text = rifleGun.GunName;
        SetEmptyAmmoIcon(rifleGun.EmptyAmmoIcon, rifleGun.MagazineSize);
        SetAmmoIcon(rifleGun.AmmoIcon, rifleGun.MagazineSize);
        SetCurrentAmmo(rifleGun.CurrentAmmo);
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

    void OnRifleAmmoChange(object sender, AmmoChangedEventArgs e)
    {
        if (e.Item.WeaponNetworkObject.GetInstanceID() != currentItem.WeaponNetworkObject.GetInstanceID())
            return;
        SetCurrentAmmoClientRpc(e.CurrentValue, RpcTarget.Single(e.OwnerID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void SetCurrentAmmoClientRpc(int currentAmmo, RpcParams rpcParams)
    {
        SetCurrentAmmo(currentAmmo);
    }
}
