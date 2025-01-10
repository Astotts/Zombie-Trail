using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Zombie/Movement", fileName = "New Zombie Movement Stats")]
public class BaseZombieMovementStats : ScriptableObject
{
    [field: SerializeField] public float MoveSpeed { get; private set; }
}