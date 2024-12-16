using System;
using System.Collections.Generic;
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
    public void AddItemServerRpc(IItem newItem, int slot, RpcParams rpcParams) {
        ulong senderID = rpcParams.Receive.SenderClientId;
        if (slot >= INVENTORY_SIZE)
        {
            Debug.LogError("Client " + senderID + " somehow access to invalid item slot " + slot);
            return;
        }

        ItemSlot[] inventory = playerInventories[senderID];
        if (inventory[slot] == null)
            inventory[slot] = new ItemSlot();

        if (inventory[slot].item != null)
        {
            Debug.Log("Client can't pickup item with that slot because it is full");
            return;
        }

        inventory[slot].item = newItem;

        UpdateItemSlotClientRpc(newItem, slot, RpcTarget.Single(senderID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void UpdateItemSlotClientRpc(IItem item, int slotNum, RpcParams rpcParams) {
        inventory[slotNum].item = item;
    }
}

public class ItemSlot
{
    public IItem item;
    public int amount;
}

public interface IItem
{
    public void OnLeftClick();
    public void OnRightClick();
    public void OnSwapIn();
    public void OnSwapOut();
    public void OnPickUp();
    public void OnDrop();
}