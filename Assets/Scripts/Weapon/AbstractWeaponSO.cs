using UnityEngine;

public abstract class AbstractWeaponStats : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public EquipmentSlot Type { get; private set; }
}

public enum EquipmentSlot
{
    PRIMARY = 0,
    SECONDARY,
    MELEE,
    CONSUMABLE
}