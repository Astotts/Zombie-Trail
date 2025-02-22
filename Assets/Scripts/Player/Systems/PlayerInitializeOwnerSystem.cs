using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.BakingSystem)]
public partial struct InitializeOwnerCharacterSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkId>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new(Unity.Collections.Allocator.Temp);

        foreach (
            var (transform, entity)
            in
            SystemAPI
                .Query<LocalTransform>()
                .WithAll<GhostOwnerIsLocal, PlayerTag>()
                .WithNone<OwnerCharacterTag>()
                .WithEntityAccess()
        )
        {
            ecb.AddComponent<OwnerCharacterTag>(entity);
        }

        ecb.Playback(state.EntityManager);

        ecb.Dispose();
    }
}