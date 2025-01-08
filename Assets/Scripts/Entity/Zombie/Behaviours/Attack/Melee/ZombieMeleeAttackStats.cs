using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(menuName = "Zombie/AttackStats/Melee", fileName = "New Zombie Melee Stats")]
public class ZombieMeleeAttackStats : AbstractAttackStats
{
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public Vector2 HitboxCenter { get; private set; }
    [field: SerializeField] public Vector2 HitboxExtends { get; private set; }
}