using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
[UpdateAfter(typeof(PlayerMoveSystem))]
partial struct ItemPickUpSystem : ISystem
{
    EntityQuery playerQuery;
    CollisionFilter targetItemFilter;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<ItemTag>();

        playerQuery = SystemAPI
            .QueryBuilder()
            .WithAll<LocalTransform, PlayerPickUpItemInput>()
            .Build();
        
        targetItemFilter = new()
        {
            BelongsTo = 1 << 6, // Player
            CollidesWith = 1 << 7 // Not yet picked up Items
        };
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NetworkTime networkTime = SystemAPI.GetSingleton<NetworkTime>();
        if (!networkTime.IsFirstTimeFullyPredictingTick)
            return;
        new PlayerPickUpJob
        {
            TargetItemFilter = targetItemFilter,
            World = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld
        }.ScheduleParallel(playerQuery);
    }

    [BurstCompile]
    public partial struct PlayerPickUpJob : IJobEntity
    {
        [ReadOnly] public CollisionFilter TargetItemFilter;
        [ReadOnly] public CollisionWorld World;

        public void Execute(in LocalTransform localTransform, in PlayerPickUpItemInput pickUpInput)
        {
            if (!pickUpInput.Value.IsSet)
                return;
            
        }
    }
}
