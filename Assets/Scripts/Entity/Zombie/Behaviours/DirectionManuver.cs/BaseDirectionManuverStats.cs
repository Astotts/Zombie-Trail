
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Zombie/DirectionManuver/Melee")]
public class BaseDirectionManuverStats : ScriptableObject
{
    [field: SerializeField] public float SearchCooldown { get; private set; }
    [field: SerializeField] public float RotateSpeed { get; private set; }
}