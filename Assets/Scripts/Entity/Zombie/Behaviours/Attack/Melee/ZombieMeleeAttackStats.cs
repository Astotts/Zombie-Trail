using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Zombie/Attack/Melee", fileName = "New Zombie Melee Stats")]
public class ZombieMeleeAttackStats : ScriptableObject
{
    [field: SerializeField] public float AttackTime { get; private set; }
    [field: SerializeField] public float AnimationAttackTime { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public Vector2 HitboxCenter { get; private set; }
    [field: SerializeField] public Vector2 HitboxExtends { get; private set; }
}