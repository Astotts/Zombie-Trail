using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(menuName = "ZombieStats", fileName = "New Zombie")]
public class ZombieStats : ScriptableObject
{
    [field: SerializeField] public AnimatorController Animation { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
}
