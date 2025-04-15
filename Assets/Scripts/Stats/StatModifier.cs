using System;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

[Serializable]
public class StatModifier
{
    [field: SerializeField] public ModifierType Type { get; private set; }
    [field: SerializeField] public float Value { get; private set; }
    [field: SerializeField] public int Order { get; private set; }
    [field: SerializeField] public object Source { get; private set; }
}

public enum ModifierType
{
    FLAT = 100,
    PERCENT_ADD = 200,
    PERCENT_MULT = 300
}