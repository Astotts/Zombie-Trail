using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEngine;
public class ThrownProjectileMovement : NetworkBehaviour
{
    [SerializeField] NetworkObject networkObject;
    [SerializeField] ParticleSystem orbParticle;
    [SerializeField] ParticleSystem splashParticle;

    ThrownProjectileStats stats;
    private Vector2 targetPosition;
    float currentSpeed;
    private float maxRelativeHeight;
    Vector2 range;
    Vector2 initialPosition;

    float nextYTrajectoryPosition;
    float nextXTrajectoryPosition;
    float nextPosYCorrectionAbsolute;
    float nextPosXCorrectionAbsolute;

    bool isReached;

    void OnDrawGizmos()
    {
        if (stats == null)
            return;
        Gizmos.DrawWireCube(targetPosition + stats.HitboxOrigin, stats.HitboxExtends);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
    }

    void FixedUpdate()
    {
        if (isReached)
            return;

        if (Mathf.Abs(range.normalized.x) < Mathf.Abs(range.normalized.y))
        {
            // Projectile will be curved on the X axis
            if (range.y < 0)
            {
                // Target is located under shooter
                currentSpeed = -currentSpeed;
            }
            UpdatePositionWithXCurve();
        }
        else
        {
            // Projectile will be curved on the Y axis
            if (range.x < 0)
            {
                // Target is located behind shooter
                currentSpeed = -currentSpeed;
            }
            UpdatePositionWithYCurve();
        }

        if (Vector2.Distance(transform.position, targetPosition) < stats.DistanceAroundTargetToStop)
            OnReach();
    }

    void OnReach()
    {
        isReached = true;
        transform.position = targetPosition;
        orbParticle.Stop();

        splashParticle.Play();
    }

    private void UpdatePositionWithXCurve()
    {
        float nextPosY = transform.position.y + currentSpeed * Time.deltaTime;
        float nextPosYNormalized = (nextPosY - initialPosition.y) / range.y;

        float nextPosXNormalized = stats.MovementCurve.Evaluate(nextPosYNormalized);
        nextXTrajectoryPosition = nextPosXNormalized * maxRelativeHeight;

        float nextPosXCorrectionNormalized = stats.CorrectionCurve.Evaluate(nextPosYNormalized);
        nextPosXCorrectionAbsolute = nextPosXCorrectionNormalized * range.x;

        if (range.x > 0 && range.y > 0)
        {
            nextXTrajectoryPosition = -nextXTrajectoryPosition;
        }

        if (range.x < 0 && range.y < 0)
        {
            nextXTrajectoryPosition = -nextXTrajectoryPosition;
        }

        float nextPosX = initialPosition.x + nextXTrajectoryPosition + nextPosXCorrectionAbsolute;

        Vector2 newPosition = new(nextPosX, nextPosY);

        CalculateNextProjectileSpeed(nextPosYNormalized);

        transform.position = newPosition;
    }


    private void UpdatePositionWithYCurve()
    {
        float nextPosX = transform.position.x + currentSpeed * Time.deltaTime;
        float nextPosXNormalized = (nextPosX - initialPosition.x) / range.x;

        float nextPosYNormalized = stats.MovementCurve.Evaluate(nextPosXNormalized);
        nextYTrajectoryPosition = nextPosYNormalized * maxRelativeHeight;

        float nextPosYCorrectionNormalized = stats.CorrectionCurve.Evaluate(nextPosXNormalized);
        nextPosYCorrectionAbsolute = nextPosYCorrectionNormalized * range.y;

        float nextPosY = initialPosition.y + nextYTrajectoryPosition + nextPosYCorrectionAbsolute;

        Vector2 newPosition = new(nextPosX, nextPosY);

        CalculateNextProjectileSpeed(nextPosXNormalized);

        transform.position = newPosition;
    }

    private void CalculateNextProjectileSpeed(float nextPosXNormalized)
    {
        float nextMoveSpeedNormalized = stats.SpeedCurve.Evaluate(nextPosXNormalized);

        currentSpeed = nextMoveSpeedNormalized * stats.MaxSpeed;
    }

    public void Despawn()
    {
        Destroy(gameObject);
        // GameObject prefab = stats.Prefab;

        // NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject, prefab);

        // networkObject.Despawn();
    }

    public void InitializeInfo(ThrownProjectileStats stats, Vector2 targetPosition)
    {
        this.stats = stats;
        this.targetPosition = targetPosition;
        initialPosition = transform.position;

        float xDistanceToTarget = targetPosition.x - transform.position.x;
        maxRelativeHeight = Mathf.Abs(xDistanceToTarget) * stats.MaxHeight;

        range = targetPosition - initialPosition;
    }
}
