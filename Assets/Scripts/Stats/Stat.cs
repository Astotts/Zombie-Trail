using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using TMPro.EditorUtilities;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine.UIElements.Experimental;

[Serializable]
public class Stat
{
    public readonly float baseValue;
    public float Value {
        get {
            if (isDirty || lastBaseValue != baseValue) {
                lastBaseValue = baseValue;
                value = CalculateFinalValue();
                isDirty = false;
            }
            return value;
        }
    }
    private readonly List<StatModifier> statModifiers;
    private readonly ReadOnlyCollection<StatModifier> StatModifiers;

    private float lastBaseValue;
    private bool isDirty;
    private float value;

    public Stat()
    {
        statModifiers = new();
        StatModifiers = statModifiers.AsReadOnly();
    }
    
    public Stat(float baseValue) : this()
    {
        this.baseValue = baseValue;
    }
    
    public void AddModifier(StatModifier mod) {
        statModifiers.Add(mod);
        statModifiers.Sort(CompareModifierOrder);
        isDirty = true;
    }

    private int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.Order < b.Order)
            return -1;
        else if (a.Order > b.Order)
            return 1;

        return 0;
    }

    public bool RemoveModifier(StatModifier mod) {
        isDirty = true;
        return statModifiers.Remove(mod);
    }

    public bool RemoveModifierFromSources(object source) {
        bool removed = false;
        for (int i = statModifiers.Count - 1; i >= 0; i--)
        {
            if (statModifiers[i].Source == source)
            {
                isDirty = true;
                removed = true;
                statModifiers.RemoveAt(i);
            }
        }
        return removed;
    }

    float CalculateFinalValue() {
        float finalValue = baseValue;
        float sumPercentAdd = 0;
        
        for (int i = 0; i < statModifiers.Count; i++) {
            StatModifier mod = statModifiers[i];
            
            switch (mod.Type) {
                case ModifierType.FLAT:
                    finalValue += mod.Value;
                    break;
                case ModifierType.PERCENT_ADD:
                    sumPercentAdd += mod.Value;
                    if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != ModifierType.PERCENT_ADD)
                    {
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }
                    break;
                case ModifierType.PERCENT_MULT:
                    finalValue *= 1 + mod.Value;
                    break;
            }
        }
        return (float)Math.Round(finalValue, 4);
    }
}