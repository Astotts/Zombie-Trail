using Unity.Entities;

[TemporaryBakingType]
public struct InitialInertia : IComponentData
{
    public bool InfX;
    public bool InfY;
    public bool InfZ;
}
