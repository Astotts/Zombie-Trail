
using UnityEngine;
using UnityEngine.Assertions.Comparers;

[CreateAssetMenu(menuName = "Stats/Zombie/Attack/Blast", fileName = "New Blast Attack Stats")]
public class ZombieBlastAttackStats : ScriptableObject
{
    [field: SerializeField] public float AttackTime { get; private set; }
    [field: SerializeField] public float AnimationAttackTime { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float Force { get; private set; }
    [field: SerializeField] public float Cooldown { get; private set; }
    [field: SerializeField] public LayerMask LayerToDetect { get; private set; }
    [field: SerializeField] public LayerMask LayerToAttack { get; private set; }
    [field: SerializeField] public Vector2 HitboxCenter { get; private set; }
    [field: SerializeField] public Vector2 HitboxExtends { get; private set; }
    [field: SerializeField] public GameObject ParticleOnBlast { get; private set; }
}