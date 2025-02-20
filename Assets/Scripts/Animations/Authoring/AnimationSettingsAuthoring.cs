using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

class AnimationSettingsAuthoring : MonoBehaviour
{
}

class AnimationSettingsAuthoringBaker : Baker<AnimationSettingsAuthoring>
{
    public override void Bake(AnimationSettingsAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new AnimationSettings()
        {
            IdleHash =      Animator.StringToHash("idle"),
            WalkLeftHash =  Animator.StringToHash("walk_left"),
            WalkRightHash = Animator.StringToHash("walk_right"),
            WalkUpHash =    Animator.StringToHash("walk_up"),
            WalkDownHash =  Animator.StringToHash("walk_down")
        });
    }
}
