using Unity.Entities;
using Unity.Mathematics;

public struct ItemOffset : IComponentData
{
    public float2 CenterPosition;   // Position from the player
    public float2 RotatePosition;   // Position rotation
    public float EulerRotation;     // Z rotation offset
}