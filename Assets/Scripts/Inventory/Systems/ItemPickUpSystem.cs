using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
[UpdateAfter(typeof(PlayerMoveSystem))]
partial struct ItemPickUpSystem : ISystem
{
    EntityQuery playerQuery;
    CollisionFilter targetItemFilter;
    float distanceAroundPlayer;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<ItemTag>();

        playerQuery = SystemAPI
            .QueryBuilder()
            .WithAllRW<ItemSlot>()
            .WithAll<LocalTransform, PlayerPickUpItemInput, CurrentSlot>()
            .Build();
        
        targetItemFilter = new()
        {
            BelongsTo = 1 << 6, // Player
            CollidesWith = 1 << 7 // Not yet picked up Items
        };
        
        distanceAroundPlayer = 2;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NetworkTime networkTime = SystemAPI.GetSingleton<NetworkTime>();
        if (!networkTime.IsFirstTimeFullyPredictingTick)
            return;

        EntityCommandBuffer ecb = new(Allocator.TempJob);

        state.Dependency = new PlayerPickUpJob
        {
            ParallelECB = ecb.AsParallelWriter(),
            DistanceAroundPlayer = distanceAroundPlayer,
            TargetItemFilter = targetItemFilter,
            World = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld,
        }.Schedule(playerQuery, state.Dependency);

        state.Dependency.Complete();

        ecb.Playback(state.EntityManager);

        ecb.Dispose();
    }

    [BurstCompile]
    public partial struct PlayerPickUpJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ParallelECB;
        public float DistanceAroundPlayer;
        public CollisionFilter TargetItemFilter;
        public CollisionWorld World;

        public void Execute(in LocalTransform localTransform, in PlayerPickUpItemInput pickUpInput, 
            in CurrentSlot currentSlot, ref DynamicBuffer<ItemSlot> inventory, [ChunkIndexInQuery] int sortKey)
        {
            if (!pickUpInput.Value.IsSet)
                return;

            PointDistanceInput distanceAroundPlayer = new()
            {
                Position = localTransform.Position,
                MaxDistance = DistanceAroundPlayer,
                Filter = TargetItemFilter
            };

            if (!World.CalculateDistance(distanceAroundPlayer, out DistanceHit closestHit))
                return;
            
            if (inventory[currentSlot.Value].Item != Entity.Null)
                return;
            
            inventory[currentSlot.Value] = new ItemSlot { Item = closestHit.Entity };

            ParallelECB.AddComponent<PickedUpItemTag>(sortKey, closestHit.Entity);
        }
    }
}
