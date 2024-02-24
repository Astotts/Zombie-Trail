using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// InventoryManager:
/// Creation and storage of inventory is done here. 
/// </summary>
public class InventoryManager : MonoBehaviour
{

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
    Item medicalPack                = new Item("MedicalPack", 5, 0, 1, InventoryItem.MedicalPack);
    // Weapons
    Item weapon_bat                 = new Item("Bat", 1, 0, 3.4f, InventoryItem.Bat);
    Item weapon_piston              = new Item("Pistol", 1, 0, 1.5f, InventoryItem.Pistol);
    Item weapon_shotgun             = new Item("Shotgun", 1, 0, 6, InventoryItem.Shotgun);
    Item weapon_assault_rifle       = new Item("Assault Rifle", 1, 0, 7.5f, InventoryItem.AssaultRifle);

    // Items
    Item items_ammo_pistol          = new Item("Pistol Ammo", 120, 0, .001f, InventoryItem.AmmoPistol);
    Item items_ammo_assault_rifle   = new Item("Assault Rifle Ammo", 120, 0, .001f, InventoryItem.AmmoAssaultRifle);
    Item coin                       = new Item("Coin", int.MaxValue, 0, 0f, InventoryItem.Coin);

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
        allItemsInGame.Add(coin);                       // 7
    }

    public void AddToInventory(ItemCount itemToAdd)
    {
        Item item = allItemsInGame.Find(x => x.itemType == itemToAdd.itemType);
        item.currentStored += itemToAdd.amount;
        if (!playersItems.Contains(item))
        {
            playersItems.Add(item);
            return;
        }
        item = playersItems.Find(x => x.itemType == itemToAdd.itemType);
        item.currentStored += itemToAdd.amount;
        UpdateWeight();
    }

    public void SubtractFromInventory(ItemCount itemToSub)
    {
        Item item = allItemsInGame.Find(x => x.itemType == itemToSub.itemType);
        item.currentStored += itemToSub.amount;
        if (item.currentStored < 0)
            item.currentStored = 0;
        if (!playersItems.Contains(item))
        {
            playersItems.Add(item);
            return;
        }
        item = playersItems.Find(x => x.itemType == itemToSub.itemType);
        item.currentStored += itemToSub.amount;
        if (item.currentStored < 0)
            item.currentStored = 0;
        UpdateWeight();
    }

    void UpdateWeight()
    {
        float totalWeight = 0;
        foreach (Item i in playersItems)
            totalWeight += i.weight * i.currentStored;
        currentWeight = totalWeight;
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
    public string itemName;       // Item's name
    public int maxStorable;        // Max amount storable
    public int currentStored;      // The player's current amount of this item on hand
    public float weight;         // Weight of the item
    public InventoryItem itemType;

    public Item(string itemNameParam, int maxStorableParam, int currentStoredParam, float weightOfTheObject, InventoryItem itemType)
    {
        this.itemName       = itemNameParam;
        this.maxStorable    = maxStorableParam;
        this.currentStored  = currentStoredParam;
        this.weight         = weightOfTheObject;
        this.itemType       = itemType;
    }
}


public enum InventoryItem : int
{
    MedicalPack = 0,    // 0
    Bat,                // 1
    Pistol,             // 2
    Shotgun,            // 3
    AssaultRifle,       // 4
    AmmoPistol,         // 5
    AmmoAssaultRifle,    // 6
    Coin
}