using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
partial struct ItemSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NetworkTime networkTime = SystemAPI.GetSingleton<NetworkTime>();
        EntityCommandBuffer ecb = new(Allocator.Temp);
        if (!networkTime.IsFirstTimeFullyPredictingTick)
            return;

        foreach (
            var (playerTransform, itemSpawnInput, ghostOwner)
            in
            SystemAPI
                .Query<RefRO<LocalTransform>, RefRO<PlayerSpawnItemInput>, RefRO<GhostOwner>>()
                .WithAll<Simulate>()
        )
        {
            if (!itemSpawnInput.ValueRO.Value.IsSet)
                continue;
            Entity item = SystemAPI.GetSingleton<EntitiesReferences>().Item;
            ecb.SetComponent(item, new GhostOwner { NetworkId = ghostOwner.ValueRO.NetworkId });
            ecb.AddComponent<NeedDestroyComponentServerSide>(item);

           ecb.Instantiate(item);
        }

        ecb.Playback(state.EntityManager);

        ecb.Dispose();
    }
}
