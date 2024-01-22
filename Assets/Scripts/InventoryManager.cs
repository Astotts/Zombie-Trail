using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// InventoryManager:
/// Creation and storage of inventory is done here. 
/// </summary>
public class InventoryManager : MonoBehaviour
{
    // Store all the items in the game on this list
    List<Item> allItemsInGame = new List<Item>();           // Store all the items in the game here, to pull for reference if needed
    // Keeping track of the player's inventory
    List<Item> playersItems = new List<Item>();             // List to stores the items that the player is carrying
    float maxWeight         = 15;                           // Max amount of weight the player can carry (start at a low default, and can work up from there for weight carry)
    float currentWeight     = 0;                            // The amount of weight the player is currently carrying

    private void Awake()
    {
        // Add all the inventory items to the list for the game to use

        // Medical
        Item medicalPack = new Item("MedicalPack", 5, 0);
        // Weapons
        Item weapon_bat = new Item("Bat", 1, 0);
        Item weapon_piston = new Item("Pistol", 1, 0);
        Item weapon_shotgun = new Item("Shotgun", 1, 0);
        Item weapon_assault_rifle = new Item("Assault Rifle", 1, 0);

        // Items
        Item items_ammo_pistol = new Item("Pistol Ammo", 120, 0);
        Item items_ammo_assault_rifle = new Item("Assault Rifle Ammo", 120, 0);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}

/// <summary>
/// Item: (Keeps the properties of and individual item)
///
///          itemName -         The name of the item
///          maxStorable -      The max amount allowed to be stored
///          currentStored -    The # amount that the player has currently on them
/// </summary>
public class Item : MonoBehaviour
{
    // The following properties will be applied during the start up of this script
    string itemName = "";       // Item's name
    int maxStorable = 0;        // Max amount storable
    int currentStored = 0;      // The player's current amount of this item on hand

    public Item(string itemNameParam, int maxStorableParam, int currentStoredParam)
    {
        this.itemName = itemNameParam;
        this.maxStorable = maxStorableParam;
        this.currentStored = currentStoredParam;
    }
}
