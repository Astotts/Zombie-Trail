using Unity.Entities;
using UnityEngine;

class CameraSettingsAuthoring : MonoBehaviour
{
    public Vector3 offset;
}

class CameraSettingsAuthoringBaker : Baker<CameraSettingsAuthoring>
{
    public override void Bake(CameraSettingsAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new CameraSettings
        {
            Offset = authoring.offset
        });
    }
}
