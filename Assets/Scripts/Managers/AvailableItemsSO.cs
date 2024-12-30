using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "AvailableItems", fileName = "AvailableItems")]
public class AvailableItemsSO : ScriptableObject
{
    [SerializeField] public List<GameObject> itemGOList;

    private Dictionary<string, GameObject> itemPrefabMap = new();

    void Awake()
    {
        foreach (GameObject itemGO in itemGOList)
        {
            IItem item = itemGO.GetComponent<IItem>();
            itemPrefabMap.Add(item.Id, itemGO);
        }
    }

    public GameObject GetItemPrefabFromID(string id)
    {
        return itemPrefabMap[id];
    }
}
