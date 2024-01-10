using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// InventoryController: Here to keep track of all the inventory in the game
public class InventoryController : MonoBehaviour
{
    // Can use enum to work with a list/array when interacting with inventory items
    enum InventoryItem { Bat, Pistol, Shotgun, AssaultRifle}

    // Space in the inventory
    [SerializeField] int numberOfInventorySlots = 6;    // I'm just messing around
    [SerializeField] Item medicalHealthPack;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddToInventory(Item item, int amountToAdd)
    {
        // Add to player's inventory #
    }

    void SubtractFromInventory(Item item, int amountToSubtract)
    {
        // Subtract from player's inventory #
    }

    // Check if the amaount is there to give or not +/-


    public class Item : MonoBehaviour
    {
        string itemName;        // Item name
        int itemMaxCount;       // Maximum amount of this item you can hold
        float itemWeight;

        public Item(string itemName)
        {
            this.itemName = itemName;
        }
    }

    // Inventory item properties - How should we o
    
}
