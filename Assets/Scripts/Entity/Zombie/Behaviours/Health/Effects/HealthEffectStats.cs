using System;
using JetBrains.Annotations;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "Zombie/Health/VFXStats", fileName = "New Health VFX Stats")]
public class HealthVFXStats : ScriptableObject
{
    [field: SerializeField] public float SpriteFlashTime { get; private set; }
    [field: SerializeField] public float HealthBarSingleFlashTime { get; private set; }
    [field: SerializeField] public float HealthBarFlashCycles { get; private set; }
    [field: SerializeField] public float HealthBarDecreasingTime { get; private set; }
    [field: SerializeField] public float HealthBarTimeBeforeFade { get; private set; }
    [field: SerializeField] public float HealthBarFadeTime { get; private set; }
}