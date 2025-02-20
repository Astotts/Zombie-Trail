using Unity.Entities;
using UnityEngine;

struct AnimationClipBlob
{
    public int Id;
    public BlobArray<float> FrameDurations;
    public BlobArray<int> FrameSprites;
}

struct AnimationBlob
{
    public BlobArray<AnimationClipBlob> animationClips;
}

struct SpriteAnimation : IComponentData
{
    public BlobAssetReference<AnimationBlob> Blob;
}