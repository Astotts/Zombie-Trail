
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Zombie/DirectionManuver/Ranged")]
public class RangedDirectionManuverStats : ScriptableObject
{
    [field: SerializeField] public float SearchCooldown { get; private set; }
    [field: SerializeField] public float RotateSpeed { get; private set; }
    [field: SerializeField] public float DistanceToKeep { get; private set; }
}