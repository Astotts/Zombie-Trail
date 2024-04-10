using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpResource : MonoBehaviour
{
    [SerializeField] int min;
    [SerializeField] int max;
    [SerializeField] InventoryItem itemType;
    ItemCount itemCount;

    void Awake()
    {
        int amount = Random.Range(min,max);
        itemCount = new ItemCount(itemType, amount);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.instance.AddToInventory(itemCount);
            Destroy(this.gameObject);
        }
    }
}
