using JetBrains.Annotations;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial class CameraFollowPlayer : SystemBase
{
    Transform cameraTransform;

    protected override void OnCreate()
    {
        RequireForUpdate<PlayerTag>();
        RequireForUpdate<CameraSettings>();
    }

    protected override void OnStartRunning()
    {
        cameraTransform = Camera.main.transform;
    }

    protected override void OnUpdate()
    {
        CameraSettings cameraSettings = SystemAPI.GetSingleton<CameraSettings>();
        foreach 
        (
            var playerTransform
            in
            SystemAPI
                .Query<RefRO<LocalTransform>>()
                .WithAll<PlayerTag, GhostOwnerIsLocal>()
        )
        {
            cameraTransform.position = playerTransform.ValueRO.Position + cameraSettings.Offset;
        }
    }
}