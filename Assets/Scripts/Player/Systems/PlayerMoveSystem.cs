using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
partial struct PlayerMoveSystem : ISystem
{
    EntityQuery playerQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();      
        playerQuery = SystemAPI
            .QueryBuilder()
            .WithAllRW<LocalTransform, MoveDirection>()
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
            MovingLookup = SystemAPI.GetComponentLookup<MovingTag>(false)
        }.ScheduleParallel(playerQuery);
    }

    [BurstCompile]
    private partial struct MoveJob : IJobEntity
    {
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public float MoveSpeed;
        [NativeDisableParallelForRestriction] public ComponentLookup<MovingTag> MovingLookup;

        public void Execute(Entity entity, ref LocalTransform playerTransform, ref MoveDirection moveDirection, in PlayerMoveInput moveInput, [EntityIndexInQuery] int index)
        {
            if (moveInput.Value.x == 0 && moveInput.Value.y == 0)
            {
                MovingLookup.SetComponentEnabled(entity, false);
                return;
            }

            moveDirection.Value = moveInput.Value;
            playerTransform.Position += MoveSpeed * DeltaTime * new float3(moveInput.Value.x, moveInput.Value.y, 0);
            MovingLookup.SetComponentEnabled(entity, true);
        }
    }
}
