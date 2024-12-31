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
        if (playerData.Position == null)
            return;

        float[] jsonVector = playerData.Position;
        transform.position = new(jsonVector[0], jsonVector[1], jsonVector[2]);

        float[] jsonQuaternion = playerData.Rotation;
        transform.rotation = new(jsonQuaternion[0], jsonQuaternion[1], jsonQuaternion[2], jsonQuaternion[3]);

        playerHealth.LoadData(playerData);
        playerInventory.LoadData(playerData);
    }

    public void SaveData(ref PlayerData playerData)
    {
        Vector3 currentPosition = transform.position;
        float[] jsonPosition = { currentPosition.x, currentPosition.y, currentPosition.z };
        playerData.Position = jsonPosition;

        Quaternion currentRotation = transform.rotation;
        float[] jsonRotation = { currentRotation.x, currentRotation.y, currentPosition.z, currentRotation.w };
        playerData.Rotation = jsonRotation;

        playerHealth.SaveData(ref playerData);
        playerInventory.SaveData(ref playerData);
    }
}

public class PlayerData
{
    public string Name { get; set; }
    public float[] Position { get; set; }
    public float[] Rotation { get; set; }
    public float CurrentHealth { get; set; }
    public ItemData[] Inventory { get; set; }
}
