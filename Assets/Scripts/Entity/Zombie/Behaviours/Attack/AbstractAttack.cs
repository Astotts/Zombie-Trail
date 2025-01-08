using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public abstract class AbstractAttack : NetworkBehaviour
{
    public abstract AbstractAttackStats Stats { get; }
    public abstract bool CanAttack();
    public abstract void Attack();
}

public abstract class AbstractAttackStats : ScriptableObject
{
    [field: SerializeField] public float AttackTime { get; private set; }
}