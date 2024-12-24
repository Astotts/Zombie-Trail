using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.Search;
using UnityEditor.U2D.Sprites;
using UnityEngine;

/// <summary>
/// InventoryManager:
/// Creation and storage of inventory is done here. 
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static int INVENTORY_SIZE = 4;
    public static InventoryManager Instance { get; private set; }
    public event EventHandler<ItemSwappedEventArgs> OnItemSwapEvent;
    public event EventHandler<ItemPickedUpEventArgs> OnItemPickedUpEvent;
    public event EventHandler<ItemDroppedEventArgs> OnItemDroppedEvent;
    [SerializeField] private AvailableItems availableItems;

    private readonly IItem[] inventory = new IItem[INVENTORY_SIZE];
    private NetworkObject player;

    void Awake()
    {
        Instance = this;
        NetworkManager.Singleton.OnClientConnectedCallback += GetPlayerNetworkObject;
    }

    void Start()
    {
        player = NetworkManager.Singleton.LocalClient.PlayerObject;
    }

    void GetPlayerNetworkObject(ulong clientID)
    {
        player = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (player != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= GetPlayerNetworkObject;
    }

    public IItem GetItem(int slot)
    {
        return inventory[slot];
    }

    public void PickUpItem(IItem item, int slot)
    {
        if (inventory[slot] != null)
            DropItem(slot);

        inventory[slot] = item;
        PickUpItemServerRpc(item.WeaponNetworkObject);


        ItemSwappedEventArgs swappedEventArgs = new()
        {
            PreviousItem = null,
            PreviousSlot = 0,
            CurrentItem = item,
            CurrentSlot = slot
        };

        OnItemSwapped(swappedEventArgs);

        ItemPickedUpEventArgs pickedUpEventArgs = new()
        {
            Item = item,
            Slot = slot
        };
        OnItemPickedUp(pickedUpEventArgs);
    }

    public void DropItem(int slot)
    {
        IItem item = inventory[slot];
        if (item == null)
            return;
        inventory[slot] = null;
        DropItemServerRpc(item.WeaponNetworkObject);

        ItemDroppedEventArgs eventArgs = new()
        {
            Item = item,
            Slot = slot
        };

        OnItemDropped(eventArgs);
    }

    public void LeftCLickItem(int slot)
    {
        IOnLeftClickEffectItem itemWithLeftClickEffect = (IOnLeftClickEffectItem)inventory[slot];
        if (itemWithLeftClickEffect == null)
            return;

        itemWithLeftClickEffect.OnLeftClick(player);
    }

    public bool SwapItem(int previousSlot, int newSlot)
    {
        if (inventory[newSlot] == null)
            return false;

        IItem previousItem = inventory[previousSlot];
        IItem newItem = inventory[newSlot];

        previousItem.WeaponNetworkObject.gameObject.SetActive(false);
        newItem.WeaponNetworkObject.gameObject.SetActive(true);

        ItemSwappedEventArgs eventArgs = new()
        {
            PreviousItem = previousItem,
            PreviousSlot = previousSlot,
            CurrentItem = newItem,
            CurrentSlot = newSlot
        };

        OnItemSwapped(eventArgs);

        return true;
    }

    [Rpc(SendTo.Server)]
    void DropItemServerRpc(NetworkObject networkObject)
    {
        networkObject.TrySetParent((Transform)null, true);
        networkObject.gameObject.layer = LayerMask.NameToLayer("PickUpRaycast");
        networkObject.gameObject.SetActive(true);
    }

    [Rpc(SendTo.Server)]
    void PickUpItemServerRpc(NetworkObject newItem)
    {
        newItem.TrySetParent(player.transform, false);
        newItem.gameObject.layer = LayerMask.NameToLayer("IgnorePickUpRaycast");
        newItem.transform.localPosition = Vector2.zero;
    }

    #region ItemSwapEvent

    void OnItemSwapped(ItemSwappedEventArgs eventArgs)
    {
        OnItemSwapEvent?.Invoke(this, eventArgs);
    }

    public class ItemSwappedEventArgs : EventArgs
    {
        public IItem PreviousItem { get; set; }
        public int PreviousSlot { get; set; }
        public IItem CurrentItem { get; set; }
        public int CurrentSlot { get; set; }
    }

    #endregion
    #region ItemPickedUpEvent

    void OnItemPickedUp(ItemPickedUpEventArgs eventArgs)
    {
        OnItemPickedUpEvent?.Invoke(this, eventArgs);
    }

    public class ItemPickedUpEventArgs : EventArgs
    {
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }

    #endregion
    #region ItemDroppedEvent

    void OnItemDropped(ItemDroppedEventArgs eventArgs)
    {
        OnItemDroppedEvent?.Invoke(this, eventArgs);
    }

    public class ItemDroppedEventArgs : EventArgs
    {
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }
    #endregion
}

public interface IItem
{
    string ItemName { get; }
    NetworkObject WeaponNetworkObject { get; }
    Sprite Icon { get; }
    Sprite WeaponSprite { get; }
    int CurrentUses { get; }
    int Capacity { get; }
}

public interface IOnLeftClickEffectItem
{
    void OnLeftClick(NetworkObject player);
}
public interface IOnRightClickEffectItem
{
    void OnRightClick(NetworkObject player);
}
public interface IOnSwapInEffectItem
{
    void OnSwapIn(NetworkObject player);
}
public interface IOnSwapOutEffectItem
{
    void OnSwapOut(NetworkObject player);
}

public interface IOnPickupEffectItem
{
    void OnPickUp(NetworkObject player);
}

public interface IOnDropEffectItem
{
    void OnDrop(NetworkObject player);
}

public interface IOnReloadEffectItem
{
    event EventHandler<float> OnReloadEvent;
    void OnReload(NetworkObject player);
}