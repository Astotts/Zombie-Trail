using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(menuName = "Zombie/Stats", fileName = "New Zombie")]
public class ZombieStats : ScriptableObject
{
    [field: SerializeField] public AnimatorController Animation { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float AttackSpeed { get; private set; }
    [field: SerializeField] public float AttackTime { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }
    [field: SerializeField] public float SearchInterval { get; private set; }
    [field: SerializeField] public Vector2 HitboxCenter { get; private set; }
    [field: SerializeField] public Vector2 HitboxExtends { get; private set; }
}
