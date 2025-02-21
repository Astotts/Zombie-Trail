using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
[UpdateBefore(typeof(PhysicsInitializeGroup))]
partial struct PlayerMoveSystem : ISystem
{
    EntityQuery playerQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();      
        playerQuery = SystemAPI
            .QueryBuilder()
            .WithAllRW<PhysicsVelocity>()
            .WithAll<PlayerMoveInput, Simulate>()
            .Build();  
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PlayerStats stats = SystemAPI.GetSingleton<PlayerStats>();

        new MoveJob
        {
            MoveSpeed = stats.MoveSpeed,
            DeltaTime = SystemAPI.Time.DeltaTime,
        }.ScheduleParallel(playerQuery);
    }

    [BurstCompile]
    private partial struct MoveJob : IJobEntity
    {
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public float MoveSpeed;

        public void Execute(ref PhysicsVelocity physicsVelocity, in PlayerMoveInput moveInput)
        {
            physicsVelocity.Angular = float3.zero;
            physicsVelocity.Linear = MoveSpeed * new float3(moveInput.Value.x, moveInput.Value.y, 0);
        }
    }
}
