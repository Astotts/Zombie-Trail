using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ItemManager : NetworkBehaviour
{
    public static ItemManager Instance { get; private set; }
    [SerializeField] AvailableItems availableItems;

    void Awake()
    {
        Instance = this;
    }

    [Rpc(SendTo.Server)]
    public void SpawnItemServerRpc(string itemID, Vector2 position)
    {
        GameObject prefab = availableItems.GetItemFromID(itemID);

        NetworkObject networkObject = NetworkObjectPool.Singleton.GetNetworkObject(prefab, position, Quaternion.identity);

        networkObject.Spawn();
    }
}

[CreateAssetMenu(menuName = "AvailableItems", fileName = "AvailableItems")]
public class AvailableItems : ScriptableObject
{
    [SerializeField] public GameObject[] items;

    private Dictionary<string, GameObject> itemMap;

    public GameObject GetItemFromID(string id)
    {
        if (itemMap[id] == null)
        {
            Debug.LogError("Item did not exist in AvailableItems ScriptableObject at " + AssetDatabase.GetAssetPath(this));
        }
        return itemMap[id];
    }
}