using System;
using System.Collections.Generic;
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
    public static event EventHandler OnItemSwapEvent;
    public static event EventHandler OnItemPickedUpEvent;
    public static event EventHandler OnItemDroppedEvent;
    [SerializeField] private AvailableItems availableItems;

    private readonly IItem[] inventory = new IItem[INVENTORY_SIZE];
    private NetworkObject player;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        player = NetworkManager.Singleton.LocalClient.PlayerObject;
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

        ItemPickedUpEventArgs eventArgs = new()
        {
            Item = item,
            Slot = slot
        };
        OnItemPickedUp(eventArgs);
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
    void PickUpItemServerRpc(NetworkObject networkObject)
    {
        networkObject.TrySetParent(player, false);
        networkObject.gameObject.layer = LayerMask.NameToLayer("IgnorePickUpRaycast");
        networkObject.transform.localPosition = Vector2.zero;
        networkObject.gameObject.SetActive(false);
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

public interface IOnLeftClickEffect
{
    void OnLeftClick(GameObject player);
}
public interface IOnRightClickEffect
{
    void OnRightClick(GameObject player);
}
public interface IOnSwapInEffect
{
    void OnSwapIn(GameObject player);
}
public interface IOnSwapOutEffect
{
    void OnSwapOut(GameObject player);
}

public interface IOnPickupEffect
{
    void OnPickUp(GameObject player);
}

public interface IOnDropEffect
{
    void OnDrop(GameObject player);
}