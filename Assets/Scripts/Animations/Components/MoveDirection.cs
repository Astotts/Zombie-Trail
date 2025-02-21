using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

public struct MoveDirection : IComponentData
{
    public float2 Value;
}
