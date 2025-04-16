using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventories", menuName = "Scriptable Objects/player/inventories")]
public class PlayerInventoriesSO : ScriptableObject
{
    public readonly Dictionary<int, IWeapon[]> Inventories = new();
    public readonly Dictionary<int, int> CurrentSlot = new();
}