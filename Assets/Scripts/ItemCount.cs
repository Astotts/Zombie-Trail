using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Properties;
using UnityEngine;

[System.Serializable]
public class ItemCount
{
    public InventoryItem itemType = 0;
    public int amount = 0;
    public ItemCount(InventoryItem t, int a)
    {
        itemType = t;
        amount = a;
    }
}
