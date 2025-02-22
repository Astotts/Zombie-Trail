using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

public struct ItemSlot : IBufferElementData
{
    public Entity Value;
}
