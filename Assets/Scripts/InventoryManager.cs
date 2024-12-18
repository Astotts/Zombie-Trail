using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

/// <summary>
/// InventoryManager:
/// Creation and storage of inventory is done here. 
/// </summary>
public class InventoryManager : NetworkBehaviour
{
    public static int INVENTORY_SIZE = 4;
    public static InventoryManager instance;
    public static event EventHandler OnItemLeftClickEvent;
    public static event EventHandler OnItemRightClickEvent;
    public static event EventHandler OnItemSwapInEvent;
    public static event EventHandler OnItemSwapOutEvent;
    public static event EventHandler OnItemPickUpEvent;
    public static event EventHandler OnItemDropEvent;

    private Dictionary<ulong, ItemSlot[]> playerInventories;

    private ItemSlot[] inventory;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        instance = this;
        inventory = new ItemSlot[INVENTORY_SIZE];

        if (!IsHost)
            return;

        playerInventories = new();

        foreach (ulong userID in NetworkManager.Singleton.ConnectedClients.Keys)
        {
            CreatePlayerInventory(userID);
        }

        NetworkManager.Singleton.OnClientConnectedCallback += CreatePlayerInventory;
    }

    void CreatePlayerInventory(ulong userID)
    {
        playerInventories.Add(userID, new ItemSlot[INVENTORY_SIZE]);
    }

    [Rpc(SendTo.Server)]
    public void PickUpItemServerRpc(IItem newItem, int slot, RpcParams rpcParams)
    {
        ulong senderID = rpcParams.Receive.SenderClientId;

        if (slot >= INVENTORY_SIZE)
        {
            Debug.LogError("Client " + senderID + " somehow access to invalid item slot " + slot);
            return;
        }

        ItemSlot[] inventory = playerInventories[senderID];

        // Create new item slot if item slot does not exist (Didn't pick up any item at the slot before)
        if (inventory[slot] == null)
            inventory[slot] = new ItemSlot();

        ItemSlot itemSlot = inventory[slot];

        if (newItem.IsPrimary) {
            inventory[slot].primary = newItem;
        }
        else
            inventory[slot].accessory = newItem;

        UpdateItemSlotClientRpc(newItem, slot, RpcTarget.Single(senderID, RpcTargetUse.Temp));
    }
    [Rpc(SendTo.Server)]
    public void DropItemServerRpc(int slot, RpcParams rpcParams) {
        ulong senderID = rpcParams.Receive.SenderClientId;
        ItemSlot[] inventory = playerInventories[senderID];
        if (inventory == null)
        {
            Debug.LogError("Client " + senderID + " does not have an registered inventory at server!");
            return;
        }
        // SOOOOOO, How does event trigger functions in items? OH RIGHT! You can have those items subscribe to inventory event
        // Spawn Item Here
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void UpdateItemSlotClientRpc(IItem newItem, int slot, RpcParams rpcParams)
    {
        // Don't worry about rpcParams, Netcode would use it by itself
        // This function should only sent to client that requested add item function

        if (newItem.IsPrimary)
            inventory[slot].primary = newItem;
        else
            inventory[slot].accessory = newItem;
    }

    #region ItemRightClickEvent
    
    public void OnItemRightClick(ItemRightClickEventArgs eventArgs) {
        OnItemRightClickEvent?.Invoke(this, eventArgs);
    }

    public class ItemRightClickEventArgs : EventArgs
    {
        public IItem Primary { get; set; }
        public IItem Accessory { get; set; }
    public int CurrentSlot { get; set; }
    }
    
    #endregion

    #region ItemLeftClickEvent

    public void OnItemLeftClick(ItemLeftClickEventArgs eventArgs)
    {
        OnItemLeftClickEvent?.Invoke(this, eventArgs);
    }

    public class ItemLeftClickEventArgs : EventArgs
    {
        public IItem Primary { get; set; }
        public IItem Accessory { get; set; }
        public int CurrentSlot { get; set; }
    }

    #endregion

    #region ItemSwapInEvent

    public void OnItemSwap(ItemSwapEventArgs eventArgs)
    {
        OnItemSwapInEvent?.Invoke(this, eventArgs);
    }

    public class ItemSwapEventArgs : EventArgs
    {
        public IItem PreviousItem { get; set; }
        public IItem PreviousAccessory { get; set; }
        public int PreviousSlot { get; set; }

        public IItem CurrentItem { get; set; }
        public IItem CurrentAccessory { get; set; }
        public int CurrentSlot { get; set; }
    }
    
    #endregion

    #region ItemSwapOutEvent

    public void OnItemSwapOut(ItemSwapEventArgs eventArgs) {
        OnItemSwapOutEvent?.Invoke(this, eventArgs);
    }
    
    public class ItemSwapOutEventArgs : EventArgs
    {
        public IItem PreviousItem { get; set; }
        public IItem PreviousAccessory { get; set; }
        public int PreviousSlot { get; set; }

        public IItem CurrentItem { get; set; }
        public IItem Accessory { get; set; }
        public int CurrentSlot { get; set; }
    }

    #endregion
    
    #region ItemPickUpEvent

    public void OnItemPickUp(ItemPickUpEventArgs eventArgs)
    {
        OnItemPickUpEvent?.Invoke(this, eventArgs);
    }

    public class ItemPickUpEventArgs : EventArgs
    {
        public IItem Primary { get; set; }
        public IItem Accessory { get; set; }
    }

    #endregion

    #region ItemDropEvent

    public void OnItemDrop(ItemDropEventArgs eventArgs) {
        OnItemDropEvent?.Invoke(this, eventArgs);
    }

    public class ItemDropEventArgs : EventArgs
    {
        public IItem Primary { get; set; }
        public IItem Accessory { get; set; }
    }
    #endregion
}

public class ItemSlot
{
    public IItem primary;       // Primaries such as guns, bat, etc
    public IItem accessory;     // Accessories such as ammo, wire, etc 
}

public interface IItem
{
    string ItemName { get; }
    Sprite Icon { get; }
    Sprite WeaponSprite { get; }
    bool IsPrimary { get; }
    int Amouunt { get; }
    void Update();
}

public interface IOnLeftClickEffect
{
    void OnLeftClick();
}
public interface IOnRightClickEffect
{
    void OnRightClick();
}
public interface IOnSwapInEffect
{
    void OnSwapIn();
}
public interface IOnSwapOutEffect
{
    void OnSwapOut();
}

public interface IOnPickupEffect
{
    void OnPickUp();
}

public interface IOnDropEffect
{
    void OnDrop();
}