using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Zombie/MovementStats", fileName = "New Zombie Movement Stats")]
public class ZombieMovementStats : ScriptableObject
{
    [field: SerializeField] public float MoveSpeed { get; private set; }
}