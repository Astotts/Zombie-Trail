
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[UpdateBefore(typeof(MoveAnimationSystem))]
public partial struct PlayerMoveDirectionClientSystem : ISystem
{
    EntityQuery playerQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        
        playerQuery = SystemAPI
            .QueryBuilder()
            .WithAllRW<MoveDirection>()
            .WithAll<PlayerTag, PlayerMoveDirection>()
            .Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new PlayerMoveDirectionClientJob
        {
            moveTagLookup = SystemAPI.GetComponentLookup<MovingTag>(false)
        }.ScheduleParallel(playerQuery);
    }

    [BurstCompile]
    public partial struct PlayerMoveDirectionClientJob : IJobEntity
    {
        [NativeDisableParallelForRestriction] public ComponentLookup<MovingTag> moveTagLookup;
        public void Execute(in PlayerMoveDirection playerMoveDirection, ref MoveDirection moveDirection, Entity playerEntity)
        {
            if (playerMoveDirection.Value.x == moveDirection.Value.x && playerMoveDirection.Value.y == moveDirection.Value.y)
                return;
            
            bool isMoving = playerMoveDirection.Value.x != 0 && playerMoveDirection.Value.y != 0;

            moveDirection.Value = playerMoveDirection.Value;
            moveTagLookup.SetComponentEnabled(playerEntity, isMoving);
        }
    }
}