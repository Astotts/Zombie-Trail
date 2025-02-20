using Unity.Collections;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

class SpriteAnimationAuthoring : MonoBehaviour
{
    public AnimationControllerSO controller;
}

class SpriteAnimationAuthoringBaker : Baker<SpriteAnimationAuthoring>
{
    public override void Bake(SpriteAnimationAuthoring authoring)
    {
        var customHash = new Unity.Entities.Hash128(
            (uint) authoring.controller.GetHashCode(),
            0, 0, 0
        );

        if (!TryGetBlobAssetReference(customHash, out BlobAssetReference<AnimationBlob> blobReference))
        {
            var builder = new BlobBuilder(Allocator.Temp);

            ref AnimationBlob animationBlob = ref builder.ConstructRoot<AnimationBlob>();

            AnimationControllerSO controllerSO = authoring.controller;
            int animationCount = controllerSO.AnimationClips.Length;

            BlobBuilderArray<AnimationClipBlob> clipArrayBuilder = builder.Allocate(
                ref animationBlob.animationClips,
                animationCount
            );

            for (int i = 0; i < animationCount; i++)
            {
                AnimationClip animationClipSO = controllerSO.AnimationClips[i];

                clipArrayBuilder[i] = new()
                {
                    Id = Animator.StringToHash(animationClipSO.Id)
                };

                int clipLength = animationClipSO.Clip.Durations.Length;

                BlobBuilderArray<float> durationArrayBuilder = builder.Allocate(
                    ref clipArrayBuilder[i].FrameDurations,
                    clipLength
                );

                BlobBuilderArray<int> spriteArrayBuilder = builder.Allocate(
                    ref clipArrayBuilder[i].FrameSprites,
                    clipLength
                );
                for (int j = 0; j < clipLength; j++)
                {
                    durationArrayBuilder[j] = animationClipSO.Clip.Durations[j];
                    spriteArrayBuilder[j] = Animator.StringToHash(animationClipSO.Clip.Sprites[j].name);
                }
            }

            blobReference = builder.CreateBlobAssetReference<AnimationBlob>(Allocator.Persistent);

            builder.Dispose();

            AddBlobAssetWithCustomHash(ref blobReference, customHash);
        }
        
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new SpriteAnimation() { Blob = blobReference });

        // Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new SpriteClipIndex {
            Value = 0
        });
        AddComponent(entity, new SpriteFrame {
            // Initialize the frame to the last frame because system would update it to the first frame right after
            Value = authoring.controller.AnimationClips[0].Clip.Durations.Length - 1
        });
        AddComponent<SpriteFrameElapsed>(entity);
        AddComponent<SpriteNeedUpdate>(entity);
    }
}
