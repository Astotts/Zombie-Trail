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

    IDisplayableWeapon currentItem;
    float ammoIconWidth;
    float emptyAmmoIconWidth;

    public override void OnNetworkSpawn()
    {
        if (!IsHost)
            return;
        EventHandler.OnItemSwappedEvent += OnItemSwap;
        EventHandler.OnItemPickedUpEvent += OnItemPickUp;
        EventHandler.OnItemDroppedEvent += OnItemDrop;
        EventHandler.OnAmmoChangedEvent += OnRifleAmmoChange;
        EventHandler.OnInventoryLoadedEvent += OnInventorySlotLoad;
        Debug.Log("Weapon Panel Subscribed to InventoryEvents!");

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsHost)
            return;
        EventHandler.OnItemSwappedEvent -= OnItemSwap;
        EventHandler.OnItemPickedUpEvent -= OnItemPickUp;
        EventHandler.OnItemDroppedEvent -= OnItemDrop;
        EventHandler.OnAmmoChangedEvent -= OnRifleAmmoChange;
        EventHandler.OnInventoryLoadedEvent -= OnInventorySlotLoad;
        Debug.Log("Weapon Panel Unsubscribed to InventoryEvents!");

        base.OnNetworkDespawn();
    }

    // =======================
    // Server Side:
    // =====================

    private void OnItemPickUp(object sender, ItemPickedUpEventArgs e)
    {
        if (e.CurrentSlot != e.PickedUpSlot)
            return;

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

    private void OnInventorySlotLoad(object sender, InventoryLoadedEventArgs e)
    {
        if (e.CurrentSlot != e.LoadedSlot)
            return;

        HandleItem(e.PlayerID, e.Item);
    }

    void HandleItem(ulong playerID, IItem item)
    {
        if (item == null)
        {
            HideItemClientRpc(RpcTarget.Single(playerID, RpcTargetUse.Temp));
            return;
        }
        if (item.WeaponNetworkObject.TryGetComponent(out IDisplayableWeapon weapon))
        {
            currentItem = weapon;
        }
        DisplayItemClientRpc(item.WeaponNetworkObject, RpcTarget.Single(playerID, RpcTargetUse.Temp));
    }
    [Rpc(SendTo.SpecifiedInParams)]
    void HideItemClientRpc(RpcParams rpcParams)
    {
        DisableUI();
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void DisplayItemClientRpc(NetworkObjectReference clientNetworkObject, RpcParams rpcParams)
    {
        if (!clientNetworkObject.TryGet(out NetworkObject networkObject))
            return;

        if (networkObject.TryGetComponent(out IDisplayableWeapon weapon))
        {
            currentItem = weapon;
            HandleDisplableWeapon(weapon);
            EnableUI();
        }
        else
        {
            DisableUI();
        }
    }

    void HandleDisplableWeapon(IDisplayableWeapon weapon)
    {
        weaponImage.sprite = weapon.Icon;
        weaponNameTMP.text = weapon.WeaponName;
        SetEmptyAmmoIcon(weapon.EmptyAmmoIcon, weapon.MagazineSize);
        SetAmmoIcon(weapon.AmmoIcon, weapon.MagazineSize);
        SetCurrentAmmo(weapon.CurrentAmmo);
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
        if (e.Item is not IDisplayableWeapon weapon || weapon != currentItem)
            return;

        SetCurrentAmmoClientRpc(e.CurrentValue, RpcTarget.Single(e.OwnerID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void SetCurrentAmmoClientRpc(int currentAmmo, RpcParams rpcParams)
    {
        SetCurrentAmmo(currentAmmo);
    }
}

public interface IDisplayableWeapon
{
    public string WeaponName { get; }
    public int CurrentAmmo { get; }
    public int MagazineSize { get; }
    public Sprite Icon { get; }
    public Sprite AmmoIcon { get; }
    public Sprite EmptyAmmoIcon { get; }
}