using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public abstract class AbstractAttack : NetworkBehaviour
{
    public abstract float AttackTime { get; }
    public abstract float AttackAnimationTime { get; }
    public abstract bool CanAttack();
    public abstract void Attack();
}