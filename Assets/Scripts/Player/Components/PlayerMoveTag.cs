using Unity.Entities;
using Unity.NetCode;

public struct PlayerMoveTag : IComponentData, IEnableableComponent
{
    [GhostField] public byte Unsued; // Have to add this because unity doesn't sync enableable components
}