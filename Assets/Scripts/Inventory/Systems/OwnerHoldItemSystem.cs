using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
[UpdateAfter(typeof(PlayerMoveSystem))]
partial struct OwnerHoldItemSystem : ISystem
{
    [ReadOnly] ComponentLookup<ItemOffset> itemOffsetLookup;
    EntityQuery playerQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ItemSlot>();

        itemOffsetLookup = SystemAPI.GetComponentLookup<ItemOffset>();

        playerQuery = SystemAPI
            .QueryBuilder()
            .WithAll<LocalTransform, PlayerMousePositionInput, CurrentSlot, ItemSlot, Simulate>()
            .Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new(Allocator.TempJob);
        itemOffsetLookup.Update(ref state);
        new PlayerHoldCurrentItemJob
        {
            ItemOffsetLookup = itemOffsetLookup,
            ParallelECB = ecb.AsParallelWriter(),
            
        }.ScheduleParallel(playerQuery);

        state.Dependency.Complete();

        ecb.Playback(state.EntityManager);

        ecb.Dispose();
    }

    [BurstCompile]
    public partial struct PlayerHoldCurrentItemJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<ItemOffset> ItemOffsetLookup;
        public EntityCommandBuffer.ParallelWriter ParallelECB;

        public void Execute(in LocalTransform playerTransform, in PlayerMousePositionInput mousePos,
            in CurrentSlot currentSlot, in DynamicBuffer<ItemSlot> inventory,
            [ChunkIndexInQuery] int sortKey)
        {
            Entity currentItemEntity = inventory[currentSlot.Value].Item;
            if (currentItemEntity == Entity.Null)
                return;
            
            // Rotation calculation
            float3 forwardDirection = new float3(mousePos.Value.x, mousePos.Value.y, 0) - playerTransform.Position;
            quaternion rotation = quaternion.LookRotationSafe(math.forward(), forwardDirection);
            rotation = math.mul(rotation, quaternion.Euler(0, 0, 90 * math.TORADIANS));

            if (!ItemOffsetLookup.TryGetComponent(currentItemEntity, out ItemOffset itemOffset))
                return;

            float3 rotatePos = new(itemOffset.RotatePosition.x, itemOffset.RotatePosition.y, 0);
            rotatePos = math.mul(rotation, rotatePos);
            float3 centerPos = new(itemOffset.CenterPosition.x, itemOffset.CenterPosition.y, 0);
            
            LocalTransform newTransform = LocalTransform.FromPositionRotation(
                playerTransform.Position + centerPos + rotatePos,
                rotation);

            ParallelECB.SetComponent(sortKey, currentItemEntity, newTransform);
        }
    }
}
