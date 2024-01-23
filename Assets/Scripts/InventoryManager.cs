using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// InventoryManager:
/// Creation and storage of inventory is done here. 
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public enum InventoryItem : int
    {
        MedicalPack = 0,    // 0
        Bat,                // 1
        Pistol,             // 2
        Shotgun,            // 3
        AssaultRifle,       // 4
        AmmoPistol,         // 5
        AmmoAssaultRifle    // 6
    }

    // Store all the items in the game on this list
    [SerializeField] List<Item> allItemsInGame = new List<Item>();              // Store all the items in the game here, to pull for reference if needed
    [SerializeField] List<Item> equipableItems = new List<Item>();              // Items that has equipable
    [SerializeField] List<Item> playersItems = new List<Item>();                // List to stores the items that the player is carrying

    float maxWeight                 = 25f;                              // Max amount of weight the player can carry (start at a low default, and can work up from there for weight carry)
    float currentWeight             = 0;                                // The amount of weight the player is currently carrying

    // Inventory Items section
    // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    // Medical
    Item medicalPack                = new Item("MedicalPack", 5, 0, 1);
    // Weapons
    Item weapon_bat                 = new Item("Bat", 1, 0, 3.4f);
    Item weapon_piston              = new Item("Pistol", 1, 0, 1.5f);
    Item weapon_shotgun             = new Item("Shotgun", 1, 0, 6);
    Item weapon_assault_rifle       = new Item("Assault Rifle", 1, 0, 7.5f);

    // Items
    Item items_ammo_pistol          = new Item("Pistol Ammo", 120, 0, .001f);
    Item items_ammo_assault_rifle   = new Item("Assault Rifle Ammo", 120, 0, .001f);

    // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        // Add all the inventory items to the list for the game to use
        // Medical
        allItemsInGame.Add(medicalPack);                // 0

        // Weapons
        allItemsInGame.Add(weapon_bat);                 // 1
        allItemsInGame.Add(weapon_piston);              // 2
        allItemsInGame.Add(weapon_shotgun);             // 3
        allItemsInGame.Add(weapon_assault_rifle);       // 4

        // Ammo
        allItemsInGame.Add(items_ammo_pistol);          // 5
        allItemsInGame.Add(items_ammo_assault_rifle);   // 6
    }

    void AddToInventory(Item itemToAdd)
    {
        // Check if this item can be added, if so, add item
    }

    void SubtractFromInventory(Item itemToAdd)
    {
        // Check if this item can be subtracted, if so, subtract item
    }
}

/// <summary>
/// Item: (Keeps the properties of and individual item)
///
///          itemName -         The name of the item
///          maxStorable -      The max amount allowed to be stored
///          currentStored -    The # amount that the player has currently on them
/// </summary>
public struct Item
{
    // The following properties will be applied during the start up of this script
    string itemName;       // Item's name
    int maxStorable;        // Max amount storable
    int currentStored;      // The player's current amount of this item on hand
    float weight;         // Weight of the item

    public Item(string itemNameParam, int maxStorableParam, int currentStoredParam, float weightOfTheObject)
    {
        this.itemName       = itemNameParam;
        this.maxStorable    = maxStorableParam;
        this.currentStored  = currentStoredParam;
        this.weight         = weightOfTheObject;
    }
}
