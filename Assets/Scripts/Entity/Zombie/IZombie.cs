using System.Numerics;
using UnityEngine;

public interface IZombie
{
    ZombieStats Stats { get; }
    EZombieType Type { get; }
    Transform Target { get; }
    bool CanAttack { get; }
    void Attack();
    bool FindTarget();
    bool MoveTowardTarget();
}

public enum EZombieType
{
    Melee,
    Ranged
}