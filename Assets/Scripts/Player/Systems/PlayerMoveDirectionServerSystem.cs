using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct PlayerMoveDirectionServerSystem : ISystem
{
    EntityQuery playerQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        
        playerQuery = SystemAPI
            .QueryBuilder()
            .WithAllRW<PlayerMoveDirection>()
            .WithAll<PlayerTag, PlayerMoveInput>()
            .Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new PlayerMoveDirectionServerJob().ScheduleParallel(playerQuery);
    }

    [BurstCompile]
    public partial struct PlayerMoveDirectionServerJob : IJobEntity
    {
        public void Execute(ref PlayerMoveDirection playerMoveDirection, in PlayerMoveInput moveInput)
        {
            if (playerMoveDirection.Value.x == moveInput.Value.x && playerMoveDirection.Value.y == moveInput.Value.y)
                return;
            playerMoveDirection.Value = moveInput.Value;
        }
    }
}