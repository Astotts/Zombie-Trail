using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

public struct PlayerMoveInput : IInputComponentData
{
    public float2 Value;
}
