using Unity.Entities;
using UnityEngine;

public class PlayerStatsAuthoring : MonoBehaviour
{
    
}

public class PlayerStatsAuthoringBaker : Baker<PlayerStatsAuthoring>
{
    public override void Bake(PlayerStatsAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new PlayerStats() {
            MoveSpeed = 2f
        });
    }
}