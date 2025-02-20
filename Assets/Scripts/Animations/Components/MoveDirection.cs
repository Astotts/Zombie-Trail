using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

// [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct MoveDirection : IComponentData
{
    public float2 Value;
}
