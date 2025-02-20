using Unity.Mathematics;
using Unity.NetCode;

public struct PlayerLookInput : IInputComponentData
{
    public float2 ScreenPos;
    public float2 WorldPos;
}