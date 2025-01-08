
using UnityEngine;

[CreateAssetMenu(menuName = "Zombie/ManuverStats")]
public class DirectionManuverStats : ScriptableObject
{
    [field: SerializeField] public float SearchCooldown { get; private set; }
    [field: SerializeField] public float RotateSpeed { get; private set; }
}