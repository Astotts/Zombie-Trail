using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.U2D.Sprites;

public class ItemData
{
    public EItemType Type { get; set; }
    public IItemStats Stats { get; set; }
}
public interface IItemStats
{
    public string UniqueID { get; set; }
}

public interface IItem
{
    Guid UniqueID { get; }
    EItemType ItemType { get; }
    NetworkObject WeaponNetworkObject { get; }
    void LoadData(ItemData data, ulong ownerID, int currentSlot, int loadedSlot);
    void SaveData(ref ItemData data);
}

public enum EItemType
{
    // If you're wondering where sniper is, it is a sniper rifle
    Rifle,
    Pistol,
    Grenade,
    Shotgun,
    Melee
}