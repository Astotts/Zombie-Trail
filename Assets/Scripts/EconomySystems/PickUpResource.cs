using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpResource : MonoBehaviour
{
    ItemCount item;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<InventoryManager>().AddToInventory(item);
            Destroy(this.gameObject);
        }
    }
}
