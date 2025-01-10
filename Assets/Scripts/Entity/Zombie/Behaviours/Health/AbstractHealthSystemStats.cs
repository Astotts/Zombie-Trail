using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Zombie/Health", fileName = "New Base Health System Stats")]
public class HealthSystemStats : ScriptableObject
{
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }
}