using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Zombie/Health/SFXStats", fileName = "New SFX Stats")]
public class HealthSFXStats : ScriptableObject
{
    [field: SerializeField] public RandomPitchSound[] OnDamagedSFX { get; private set; }
}


[Serializable]
public class RandomPitchSound
{
    [field: SerializeField] public string SoundID { get; private set; }
    [field: SerializeField] public float MinPitch { get; private set; }
    [field: SerializeField] public float MaxPitch { get; private set; }
}