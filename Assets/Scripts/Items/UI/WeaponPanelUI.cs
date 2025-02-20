using System;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;
using static EventManager;

public class WeaponPanelUI : NetworkBehaviour
{
    [SerializeField] TMP_Text weaponNameTMP;
    [SerializeField] RectTransform weaponFrameRecTransform;
    [SerializeField] RectTransform weaponRecTransform;
    [SerializeField] Image weaponImage;
    [SerializeField] RectTransform ammoAreaRectTransform;
    [SerializeField] RectTransform ammoImageRectTransform;
    [SerializeField] RectTransform emptyAmmoImageRectTransform;
    [SerializeField] Image ammoImage;
    [SerializeField] Image emptyAmmoImage;
    [SerializeField] TMP_Text magazineCounter;

    IDisplayableWeapon currentItem;
    float ammoIconWidth;
    float emptyAmmoIconWidth;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        EventManager.EventHandler.OnItemSwapPressedEvent += OnItemSwap;
        EventManager.EventHandler.OnItemPickUpPressedEvent += OnItemPickUp;
        EventManager.EventHandler.OnItemDropPressedEvent += OnItemDrop;
        EventManager.EventHandler.OnAmmoChangedEvent += OnRifleAmmoChange;
        EventManager.EventHandler.OnInventoryLoadedEvent += OnInventorySlotLoad;
        EventManager.EventHandler.OnGunReloadedEvent += OnGunReloaded;
        Debug.Log("Weapon Panel Subscribed to InventoryEvents!");

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
            return;
        EventManager.EventHandler.OnItemSwapPressedEvent -= OnItemSwap;
        EventManager.EventHandler.OnItemPickUpPressedEvent -= OnItemPickUp;
        EventManager.EventHandler.OnItemDropPressedEvent -= OnItemDrop;
        EventManager.EventHandler.OnAmmoChangedEvent -= OnRifleAmmoChange;
        EventManager.EventHandler.OnInventoryLoadedEvent -= OnInventorySlotLoad;
        EventManager.EventHandler.OnGunReloadedEvent -= OnGunReloaded;
        Debug.Log("Weapon Panel Unsubscribed to InventoryEvents!");

        base.OnNetworkDespawn();
    }

    // =======================
    // Server Side:
    // =====================

    private void OnItemPickUp(object sender, ItemPickUpPressedEventArgs e)
    {
        if (e.CurrentSlot != e.PickedUpSlot)
            return;

        HandleItem(e.PlayerID, e.Item);
    }

    private void OnItemDrop(object sender, ItemDropPressedEventArgs e)
    {
        HandleItem(e.PlayerID, null);
    }

    private void OnItemSwap(object sender, ItemSwapPressedEventArgs e)
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
        weaponNameTMP.text = weapon.WeaponName;
        SetWeaponIcon(weapon.Icon);
        SetEmptyAmmoIcon(weapon.EmptyAmmoIcon, weapon.MagazineSize);
        SetAmmoIcon(weapon.AmmoIcon, weapon.MagazineSize);
        SetCurrentAmmo(weapon.CurrentAmmo);
        SetMagazineCounter(weapon.CurrentMagazine, weapon.MaxMagazine);
    }

    void DisableUI()
    {
        // Dear future me, disabled GameObjects won't receive event
        // So I had to disable the components
        weaponNameTMP.gameObject.SetActive(false);
        weaponImage.gameObject.SetActive(false);
        ammoAreaRectTransform.gameObject.SetActive(false);
        magazineCounter.gameObject.SetActive(false);
    }
    void EnableUI()
    {
        weaponNameTMP.gameObject.SetActive(true);
        weaponImage.gameObject.SetActive(true);
        ammoAreaRectTransform.gameObject.SetActive(true);
        magazineCounter.gameObject.SetActive(true);
    }

    void SetWeaponIcon(Sprite weaponIcon)
    {
        weaponImage.sprite = weaponIcon;
        weaponImage.SetNativeSize();
        float weaponNativeHeight = weaponRecTransform.sizeDelta.y / 2;
        float weaponFrameHeight = weaponFrameRecTransform.sizeDelta.y;
        float scale = weaponFrameHeight / weaponNativeHeight;
        weaponRecTransform.localScale = new Vector3(scale, scale, scale);

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

        float rectHeight = ammoImageRectTransform.sizeDelta.y;
        float ammoAreaHeight = ammoAreaRectTransform.sizeDelta.y;

        if (rectHeight * ammoImageRectTransform.localScale.y > ammoAreaHeight)
        {
            float scale = ammoAreaHeight / rectHeight;
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
        float ammoAreaWidth = ammoAreaRectTransform.sizeDelta.x;

        // Arrange the ammo sprite if its height is outside the bound

        // Then arrange the width, making the bullet stay inside of bound (no overflow)
        if (rectWidth > ammoAreaWidth)
        {
            float scale = ammoAreaWidth / rectWidth;
            emptyAmmoImageRectTransform.localScale = new Vector3(scale, scale, scale);
        }

        float rectHeight = emptyAmmoImageRectTransform.sizeDelta.y;
        float ammoAreaHeight = ammoAreaRectTransform.sizeDelta.y;

        if (rectHeight * emptyAmmoImageRectTransform.localScale.y > ammoAreaHeight)
        {
            float scale = ammoAreaHeight / rectHeight;
            emptyAmmoImageRectTransform.localScale = new Vector3(scale, scale, scale);
        }

        emptyAmmoImageRectTransform.sizeDelta = new(rectWidth, rectHeight);
    }

    void SetMagazineCounter(int current, int capacity)
    {
        magazineCounter.text = current + "/" + capacity;
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

    private void OnGunReloaded(object sender, GunReloadedEventArgs e)
    {
        if (e.Item is not IDisplayableWeapon weapon || weapon != currentItem)
            return;

        SetMagazineCounterClientRpc(e.CurrentMagazine, e.MaxMagazine, RpcTarget.Single(e.PlayerID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SetMagazineCounterClientRpc(int currentMagazine, int capacity, RpcParams rpcParams)
    {
        SetMagazineCounter(currentMagazine, capacity);
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
    public int CurrentMagazine { get; }
    public int MaxMagazine { get; }
}