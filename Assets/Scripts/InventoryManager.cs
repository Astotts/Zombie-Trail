using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// InventoryManager:
/// Creation and storage of inventory is done here. 
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public Action<Item> OnInventoryUpdate;
    // Store all the items in the game on this list
    public static List<Item> allItemsInGame = new List<Item>();              // Store all the items in the game here, to pull for reference if needed
    [SerializeField] List<Item> equipableItems = new List<Item>();              // Items that has equipable
    public List<Item> playersItems = new List<Item>();                // List to stores the items that the player is carrying

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
        instance = this;
        // Add all the inventory items to the list for the game to use
        // Medical
        Sprite icon = Resources.Load<Sprite>("Image/Item/MedicalPack");
        allItemsInGame.Add(medicalPack.AddIcon(icon));                // 0

        // Weapons
        icon = Resources.Load<Sprite>("Image/Item/Bat");
        allItemsInGame.Add(weapon_bat.AddIcon(icon));                 // 1
        icon = Resources.Load<Sprite>("Image/Item/Pistol");
        allItemsInGame.Add(weapon_piston.AddIcon(icon));              // 2
        icon = Resources.Load<Sprite>("Image/Item/Shotgun");
        allItemsInGame.Add(weapon_shotgun.AddIcon(icon));             // 3
        icon = Resources.Load<Sprite>("Image/Item/AssaultRifle");
        allItemsInGame.Add(weapon_assault_rifle.AddIcon(icon));       // 4

        // Ammo
        icon = Resources.Load<Sprite>("Image/Item/PistolAmmo");
        allItemsInGame.Add(items_ammo_pistol.AddIcon(icon));          // 5
        icon = Resources.Load<Sprite>("Image/Item/AssaultRifleAmmo");
        allItemsInGame.Add(items_ammo_assault_rifle.AddIcon(icon));   // 6
        icon = Resources.Load<Sprite>("Image/Item/Coin");
        allItemsInGame.Add(coin.AddIcon(icon));                       // 7
    }

    public void AddToInventory(ItemCount itemToAdd)
    {
        //Debug.Log("Adding item: " + itemToAdd.itemType);
        int index = allItemsInGame.FindIndex(x => x.itemType == itemToAdd.itemType);
        //Debug.Log("Current Amount: " + allItemsInGame[index].currentStored);
        allItemsInGame[index].AddCurrentStored(itemToAdd.amount);
        if (!playersItems.Contains(allItemsInGame[index]))
        {
            playersItems.Add(allItemsInGame[index]);
            UpdateWeight();
            OnInventoryUpdate?.Invoke(allItemsInGame[index]);
            Debug.Log("Final Amount: " + allItemsInGame[index].currentStored);
            return;
        }
        index = playersItems.FindIndex(x => x.itemType == itemToAdd.itemType);
        playersItems[index].AddCurrentStored(itemToAdd.amount);
        OnInventoryUpdate?.Invoke(playersItems[index]);
        Debug.Log("Final Amount: " + allItemsInGame[index].currentStored);
        UpdateWeight();
    }

    public bool SubtractFromInventory(ItemCount itemToSub)
    {
        int index = allItemsInGame.FindIndex(x => x.itemType == itemToSub.itemType);
        allItemsInGame[index].SubCurrentStored(itemToSub.amount);
        if (!playersItems.Contains(allItemsInGame[index]) || allItemsInGame[index].currentStored < itemToSub.amount)
        {
            return false;
        }
        index = playersItems.FindIndex(x => x.itemType == itemToSub.itemType);
        playersItems[index].SubCurrentStored(itemToSub.amount);
        OnInventoryUpdate?.Invoke(playersItems[index]);
        UpdateWeight();
        return true;
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
public class Item
{
    // The following properties will be applied during the start up of this script
    public string itemName;       // Item's name
    public int maxStorable;        // Max amount storable
    public int currentStored;      // The player's current amount of this item on hand
    public float weight;         // Weight of the item
    public InventoryItem itemType;
    public Sprite icon;

    public Item(string itemNameParam, int maxStorableParam, int currentStoredParam, float weightOfTheObject, InventoryItem itemType)
    {
        this.itemName       = itemNameParam;
        this.maxStorable    = maxStorableParam;
        this.currentStored  = currentStoredParam;
        this.weight         = weightOfTheObject;
        this.itemType       = itemType;
        this.icon           = null;
    }
    public Item AddIcon(Sprite image)
    {
        this.icon = image;
        return this;
    }

    public Item SetCurrentStored(int amount)
    {
        currentStored = amount;
        return this;
    }
    public Item AddCurrentStored(int amount)
    {
        currentStored += amount;
        return this;
    }

    public Item SubCurrentStored(int amount)
    {
        currentStored -= amount;
        return this;
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