using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    
    public List<EquipmentSlot> equipmentSlots;
}

[Serializable]
public class EquipmentSlot
{
    public Image background;
    public Image ammoBackground;
    public GameObject item;
    public EItemType itemType;
}

public enum EItemType
{
    GUN,
    THOWABLE,
    MELEE,
    CONSUMABLE
}