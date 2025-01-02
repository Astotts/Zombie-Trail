using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "AvailableItems", fileName = "AvailableItems")]
public class AvailableItemsSO : ScriptableObject
{
    [SerializeField] List<GameObject> itemGOList;

    public Dictionary<string, GameObject> itemPrefabMap;

    void OnEnable()
    {
        itemPrefabMap = new();
        foreach (GameObject itemGO in itemGOList)
        {
            IItem item = itemGO.GetComponent<IItem>();
            itemPrefabMap.Add(item.Id, itemGO);
        }
    }

    public GameObject GetItemPrefabFromID(string id)
    {
        foreach (string item in itemPrefabMap.Keys)
            Debug.Log(item);
        return itemPrefabMap[id];
    }
}