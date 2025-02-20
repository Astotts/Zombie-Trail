using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
partial struct GoInGameServerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();
        state.RequireForUpdate<NetworkId>();
    }

    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = new(Allocator.Temp);

        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach (
            (
                RefRO<ReceiveRpcCommandRequest> receiveRpcCommandRequest,
                Entity entity
            )
            in
            SystemAPI
                .Query<RefRO<ReceiveRpcCommandRequest>>()
                .WithAll<GoInGameRequestRpc>()
                .WithEntityAccess()
        )
        {
            entityCommandBuffer.AddComponent<NetworkStreamInGame>(receiveRpcCommandRequest.ValueRO.SourceConnection);

            Debug.Log("Client connected to server");

            Entity playerEntity = entityCommandBuffer.Instantiate(entitiesReferences.PlayerPrefabEntity);
            entityCommandBuffer.SetComponent(playerEntity, LocalTransform.FromPosition(
                new Unity.Mathematics.float3(
                0,
                0,
                0
            )));

            NetworkId networkId = SystemAPI.GetComponent<NetworkId>(receiveRpcCommandRequest.ValueRO.SourceConnection);
            entityCommandBuffer.AddComponent(playerEntity, new GhostOwner
            {
                NetworkId = networkId.Value,
            });

            entityCommandBuffer.AppendToBuffer(receiveRpcCommandRequest.ValueRO.SourceConnection, new LinkedEntityGroup()
            {
                Value = playerEntity
            });
            entityCommandBuffer.AddComponent<NeedDestroyComponentServerSide>(playerEntity);

            entityCommandBuffer.DestroyEntity(entity);
        }
        entityCommandBuffer.Playback(state.EntityManager);

        entityCommandBuffer.Dispose();
    }
}
