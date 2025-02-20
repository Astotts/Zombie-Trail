using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial class SpriteSyncSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<SpriteNeedUpdate>();
    }
    
    protected override void OnUpdate()
    {
        foreach
        (
            var (spriteAnimation, clipIndex, frameIndex, entity)
            in
            SystemAPI
                .Query<RefRO<SpriteAnimation>, RefRO<SpriteClipIndex>, RefRO<SpriteFrame>>()
                .WithEntityAccess()
        )
        {
            if (!EntityManager.IsComponentEnabled<SpriteNeedUpdate>(entity))
                continue;
            

            ref AnimationBlob animationController = ref spriteAnimation.ValueRO.Blob.Value;
                
            var spriteId = animationController.animationClips[clipIndex.ValueRO.Value].FrameSprites[frameIndex.ValueRO.Value];

            if (SpriteSheetsVault.GetSprite(spriteId, out Sprite sprite))
            {
                var spriteRenderer = EntityManager.GetComponentObject<SpriteRenderer>(entity);
                spriteRenderer.sprite = sprite;
            }

            EntityManager.SetComponentEnabled<SpriteNeedUpdate>(entity, false);
        }
    }
}