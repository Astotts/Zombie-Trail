using System;
using System.Collections.Generic;
using Unity.Netcode;

public class ItemData
{
    public string id;
    public Dictionary<string, string> StringMap { get; set; }
    public Dictionary<string, int> IntMap { get; set; }
    public Dictionary<string, float> FloatMap { get; set; }
}

public interface IItem
{
    string Id { get; }
    NetworkObject WeaponNetworkObject { get; }
    void OnPickUp(InventoryHandler playerInventory);
    void OnDrop(InventoryHandler playerInventory);
    void LoadData(ItemData data);
    void SaveData(ref ItemData data);
}

public interface IReloadableItem
{
    event EventHandler<float> OnReloadEvent;
}