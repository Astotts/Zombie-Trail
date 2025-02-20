using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
partial struct SpriteClipSystem : ISystem
{
    EntityQuery spriteClipQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpriteAnimation>();

        spriteClipQuery = SystemAPI
            .QueryBuilder()
            .WithAllRW<SpriteFrame, SpriteFrameElapsed>()
            .WithAll<SpriteAnimation, SpriteClipIndex>()
            .Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        double elapsedTime = SystemAPI.Time.ElapsedTime;
        new SpriteClipJob()
        {
            SpriteNeedUpdateLookup = SystemAPI.GetComponentLookup<SpriteNeedUpdate>(false),
            ElapsedTime = elapsedTime
        }.ScheduleParallel(spriteClipQuery);
    }

    [BurstCompile]
    public partial struct SpriteClipJob : IJobEntity
    {
        [NativeDisableParallelForRestriction] public ComponentLookup<SpriteNeedUpdate> SpriteNeedUpdateLookup;
        public double ElapsedTime;
        public void Execute(ref SpriteFrame frame, ref SpriteFrameElapsed frameElapsed, in SpriteClipIndex clipIndex, in SpriteAnimation spriteAnimationBlob, Entity entity)
        {
            if (ElapsedTime < frameElapsed.Value)
                return;
            
            ref AnimationBlob spriteAnimation = ref spriteAnimationBlob.Blob.Value;
            ref AnimationClipBlob clip = ref spriteAnimation.animationClips[clipIndex.Value];

            int clipLength = clip.FrameDurations.Length;
            int nextFrame = (frame.Value + 1) % clipLength;

            frame.Value = nextFrame;
            frameElapsed.Value = ElapsedTime + clip.FrameDurations[nextFrame];
            SpriteNeedUpdateLookup.SetComponentEnabled(entity, true);
        }   
    }
}
