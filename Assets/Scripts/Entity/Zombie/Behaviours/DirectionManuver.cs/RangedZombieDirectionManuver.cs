using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class RangedZombieDirectionManuver : AbstractDirectionManuver
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
            return;

        this.enabled = false;
    }

    public void RotateAwayFromTarget()
    {
        if (Target == null)
            return;

        Vector2 targetPos = Target.position;
        Vector2 zombieToTargetVector = targetPos - (Vector2)transform.position;
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, -90) * zombieToTargetVector;
        float singleStep = Stats.RotateSpeed * Time.deltaTime;
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, singleStep);
    }

    public override Transform FindNearestTarget()
    {
        // IReadOnlyList<NetworkClient> playerList = NetworkManager.Singleton.ConnectedClientsList;
        List<GameObject> playerList = GameObject.FindGameObjectsWithTag("Player").ToList();

        float closestDistance = float.MaxValue;

        Target = null;

        foreach (GameObject player in playerList)
        {
            Vector2 playerPos = player.transform.position;
            float distance = Vector2.Distance(transform.position, playerPos);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                Target = player.transform;
                return Target;
            }
        }
        return null;
    }

    public override void RotateTowardTarget()
    {
        if (Target == null)
            return;
        Vector2 targetPos = Target.position;

        // Finding the closest player? Look like a working as a magnet
        // target = targetFinder.GetClosest();
        // moveDirection = (Vector2)target.position - (Vector2)characterPos.position;
        // //Debug.DrawRay(transform.position, targetWorldPos, Color.red, 0.01f);

        // vector from this object towards the target location
        Vector2 playerToTargetVector = targetPos - (Vector2)transform.position;

        // rotate that vector by 90 degrees around the Z axis
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * playerToTargetVector;

        float singleStep = Stats.RotateSpeed * Time.deltaTime;

        // get the rotation that points the Z axis forward, and the Y axis 90 degrees away from the target
        // (resulting in the X axis facing the target)
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, singleStep);
    }


    public override Vector2 GetDirection()
    {
        return gameObject.transform.rotation * Vector2.right;
    }
}