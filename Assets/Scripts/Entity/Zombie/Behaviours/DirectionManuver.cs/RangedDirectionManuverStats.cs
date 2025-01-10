
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Zombie/DirectionManuver/Ranged")]
public class RangedDirectionManuverStats : BaseDirectionManuverStats
{
    [field: SerializeField] public float DistanceToKeep { get; private set; }
}