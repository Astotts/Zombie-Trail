using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

public struct PlayerMoveDirection : IComponentData
{
    [GhostField] public float2 Value;
}