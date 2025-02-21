using NUnit.Framework.Constraints;
using Unity.Entities;
using UnityEngine;

class PlayerAuthoring : MonoBehaviour
{
}

class PlayerTagAuthoringBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new PlayerTag());
        AddComponent(entity, new PlayerMoveInput());
        AddComponent(entity, new PlayerLookInput());
        AddComponent(entity, new PlayerMoveTag());
        AddComponent(entity, new MovingTag());
        AddComponent(entity, new LookDirection());
        AddComponent(entity, new MoveDirection());
    }
}
