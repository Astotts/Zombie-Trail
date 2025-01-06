using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Zombie/HealthBarStats", fileName = "New Zombie HealthBar Stats")]
public class HealthBarStatsSO : ScriptableObject
{
    [field: SerializeField] public float SingleFlashTime { get; private set; }
    [field: SerializeField] public float FlashCycles { get; private set; }
}