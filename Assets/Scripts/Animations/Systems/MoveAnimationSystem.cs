using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
// [UpdateAfter(typeof(PredictedSimulationSystemGroup))]
partial struct MoveAnimationSystem : ISystem
{
    EntityQuery movableQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MovingTag>();

        movableQuery = SystemAPI
            .QueryBuilder()
            .WithAllRW<SpriteFrame, SpriteFrameElapsed>()
            .WithAllRW<SpriteClipIndex>()
            .WithAll<SpriteAnimation, MoveDirection, MovingTag>()
            .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)
            .Build();

        movableQuery.AddChangedVersionFilter(ComponentType.ReadOnly<MovingTag>());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new MoveAnimationJob
        {
            animationSettings = SystemAPI.GetSingleton<AnimationSettings>(),
            ElapsedTime = SystemAPI.Time.DeltaTime,
            SpriteNeedUpdateLookup = SystemAPI.GetComponentLookup<SpriteNeedUpdate>(false)
        }.ScheduleParallel(movableQuery);
    }
    
    [BurstCompile]
    public partial struct MoveAnimationJob : IJobEntity
    {
        public AnimationSettings animationSettings;
        public double ElapsedTime;
        [NativeDisableParallelForRestriction] public ComponentLookup<SpriteNeedUpdate> SpriteNeedUpdateLookup;
        
        public void Execute(ref SpriteFrame frame, ref SpriteFrameElapsed frameElapsed, ref SpriteClipIndex clipIndex,
            EnabledRefRO<MovingTag> movingTag, in SpriteAnimation spriteAnimationBlobRef,
            in MoveDirection moveDirection, Entity entity, [EntityIndexInQuery] int index)
        {
            // Calculate what clip to show`
            int clipId;
            if (!movingTag.ValueRO)
            {
                clipId = animationSettings.IdleHash;
            } else {
                float2 moveDir = math.normalize(moveDirection.Value);
                // Up and down direction
                if (math.abs(moveDir.y) > math.abs(moveDir.x))
                {
                    if (moveDir.y < 0)
                        clipId = animationSettings.WalkDownHash;
                    else
                        clipId = animationSettings.WalkUpHash;
                }
                // Left and right direction
                else
                {
                    if (moveDir.x < 0)
                        clipId = animationSettings.WalkLeftHash;
                    else
                        clipId = animationSettings.WalkRightHash;
                }
            }

            // Calculate whether to update the clip or not
            ref AnimationBlob spriteAnimation = ref spriteAnimationBlobRef.Blob.Value;

            int animationCount = spriteAnimation.animationClips.Length;
            int newClipIndex = clipIndex.Value;
            for (int i = 0; i < animationCount; i++)
            {
                if (spriteAnimation.animationClips[i].Id == clipId)
                {
                    newClipIndex = i;
                    break;
                }
            }

            if (clipIndex.Value == newClipIndex)
                return;
            
            clipIndex.Value = newClipIndex;
            ref AnimationClipBlob clipBlob = ref spriteAnimation.animationClips[newClipIndex];
            frame.Value = clipBlob.FrameDurations.Length - 1;
            frameElapsed.Value = ElapsedTime;
            SpriteNeedUpdateLookup.SetComponentEnabled(entity, true);
        }
    }
}
