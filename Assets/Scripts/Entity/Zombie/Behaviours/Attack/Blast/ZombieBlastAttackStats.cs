
using UnityEngine;

[CreateAssetMenu(menuName = "Zombie/AttackStats/Blast", fileName = "New Blast Attack Stats")]
public class ZombieBlastAttackStats : AbstractAttackStats
{
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float Force { get; private set; }
    [field: SerializeField] public LayerMask LayerToDetect { get; private set; }
    [field: SerializeField] public LayerMask LayerToAttack { get; private set; }
    [field: SerializeField] public Vector2 HitboxCenter { get; private set; }
    [field: SerializeField] public Vector2 HitboxExtends { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
}