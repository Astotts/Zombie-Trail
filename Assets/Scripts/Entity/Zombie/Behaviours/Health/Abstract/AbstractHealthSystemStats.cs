using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Zombie/Health", fileName = "New Base Health System Stats")]
public class HealthSystemStats : ScriptableObject
{
    [field: SerializeField] public float MaxHealth { get; private set; }
}