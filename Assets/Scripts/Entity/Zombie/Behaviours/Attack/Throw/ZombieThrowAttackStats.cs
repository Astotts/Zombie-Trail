using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Zombie/Attack/Throw", fileName = "New Zombie Throw Attack Stats")]
public class ZombieThrowAttackStats : AbstractAttackStats
{
    [field: SerializeField] public float Range { get; private set; }
    [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
    [field: SerializeField] public ThrownProjectileStats projectileStats { get; private set; }
}

[Serializable]
public class ThrownProjectileStats
{
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float MaxSpeed { get; private set; }
    [field: SerializeField] public float MaxHeight { get; private set; }
    [field: SerializeField] public float Force { get; private set; }
    [field: SerializeField] public float DistanceAroundTargetToStop { get; private set; }
    [field: SerializeField] public LayerMask LayerToAttack { get; private set; }
    [field: SerializeField] public Vector2 HitboxOrigin { get; private set; }
    [field: SerializeField] public Vector2 HitboxExtends { get; private set; }
    [field: SerializeField] public AnimationCurve MovementCurve { get; private set; }
    [field: SerializeField] public AnimationCurve CorrectionCurve { get; private set; }
    [field: SerializeField] public AnimationCurve SpeedCurve { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
}