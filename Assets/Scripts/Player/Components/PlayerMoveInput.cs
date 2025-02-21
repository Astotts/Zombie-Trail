using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

[GhostComponent(OwnerSendType = SendToOwnerType.SendToNonOwner, PrefabType = GhostPrefabType.AllPredicted)]
public struct PlayerMoveInput : IInputComponentData
{
    [GhostField] public float2 Value;
}
