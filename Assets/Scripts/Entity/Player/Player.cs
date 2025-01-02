using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] InventoryHandler playerInventory;
    [SerializeField] ReloadUI reloadUI;

    public ulong PlayerID
    {
        set
        {
            SetOwnerIDClientRpc(value);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetOwnerIDClientRpc(ulong clientID)
    {
        reloadUI.PlayerID = clientID;
        playerInventory.SetOwnerIDClientRpc(clientID);
    }
    public void LoadData(PlayerData playerData)
    {
        playerHealth.LoadData(playerData);
        playerInventory.LoadData(playerData);
    }

    public void SaveData(ref PlayerData playerData)
    {
        playerHealth.SaveData(ref playerData);
        playerInventory.SaveData(ref playerData);
    }
}

public class PlayerData
{
    public string Name { get; set; }
    public float CurrentHealth { get; set; }
    public int CurrentInventorySlot { get; set; }
    public ItemData[] Inventory { get; set; }
}
