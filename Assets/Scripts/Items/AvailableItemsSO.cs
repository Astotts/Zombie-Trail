using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;

[CreateAssetMenu(menuName = "AvailableItems", fileName = "AvailableItems")]
public class AvailableItemsSO : ScriptableObject
{
    [SerializeField] List<ItemPrefab> itemPrefabList;

    public Dictionary<EItemType, GameObject> itemPrefabMap;
    public Dictionary<EItemType, ScriptableObject[]> itemStatMap;

    void OnEnable()
    {
        itemPrefabMap = new();
        itemStatMap = new();
        foreach (ItemPrefab itemPrefab in itemPrefabList)
        {
            itemPrefabMap.Add(itemPrefab.ItemType, itemPrefab.Prefab);
            itemStatMap.Add(itemPrefab.ItemType, itemPrefab.ItemStats);
        }
    }

    public GameObject GetItemPrefabFromType(EItemType itemType)
    {
        return itemPrefabMap[itemType];
    }

    public int GetItemStatIndex(EItemType itemType, ScriptableObject itemSO)
    {
        ScriptableObject[] itemStats = itemStatMap[itemType];
        for (int i = 0; i < itemStats.Length; i++)
        {
            if (itemSO == itemStats[i])
                return i;
        }
        return -1;
    }

    public ScriptableObject GetItemStat(EItemType itemType, int index)
    {
        ScriptableObject[] itemStats = itemStatMap[itemType];
        return itemStats[index];
    }

    public ScriptableObject GetRandomItemStats(EItemType itemType)
    {
        ScriptableObject[] itemStats = itemStatMap[itemType];
        System.Random rand = new();
        return itemStats[rand.Next(itemStats.Length)];
    }
}

[Serializable]
public class ItemPrefab
{
    public EItemType ItemType;
    public GameObject Prefab;
    public ScriptableObject[] ItemStats;
}