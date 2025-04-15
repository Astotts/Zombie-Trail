using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemManager", menuName = "Scriptable Objects/ItemManager")]
public class ItemManager : ScriptableObject
{
    [field: SerializeField] public ItemSO[] ItemList { get; private set; }

    private readonly Dictionary<ItemSO, int> ItemIDMap = new();

    void OnEnable()
    {
        for(int i = 0; i < ItemList.Length; i++)
        {
            ItemIDMap.Add(ItemList[i], i);
        }
    }

    int GetItemIndex(ItemSO itemSO)
    {
        return ItemIDMap[itemSO];
    }

    ItemSO GetItemFromIndex(int i)
    {
        return ItemList[i];
    }
}
