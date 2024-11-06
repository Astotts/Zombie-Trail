using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GenericEnemy : NetworkBehaviour
{

    //Unit Movement
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float moveSpeed;
    [SerializeField] Rigidbody2D unitRB;

    //Tracking Destination
    [SerializeField] GetClosestTargets targetFinder;
    private Transform target;
    [SerializeField] WeaponsClass weapon;

    //Animation
    [SerializeField] Animator animator;
    private Vector2 lastPos;

    public override void OnNetworkSpawn()
    {
        if (!IsHost)
        {
            enabled = false;
            return;
        }
        base.OnNetworkSpawn();
    }

    void FixedUpdate()
    {
        transform.position = unitRB.transform.position;
        target = targetFinder.GetClosest();

        //Animator Variables
        Vector2 distTraveled = (Vector2)transform.position - lastPos;
        lastPos = (Vector2)transform.position;
        animator.SetFloat("X", distTraveled.x);
        animator.SetFloat("Y", distTraveled.y);

        MoveTo(target.position);
        //Debug.Log(targetFinder.GetDistance());
        if (targetFinder.GetDistance() < weapon.range)
        {
            weapon.Attack();
        }
    }

    void MoveTo(Vector2 position)
    {
        Vector2 moveDirection = unitRB.position - position;
        RotateTowards(moveDirection);
        //ToDo: Feature to speed up and slow down
        Vector2 newPos = moveSpeed * Time.fixedDeltaTime * (Vector2)transform.up + unitRB.position;
        unitRB.MovePosition(newPos);
        // Debug.Log(unitRB.collisionDetectionMode);
    }

    void RotateTowards(Vector2 moveDirection)
    {
        moveDirection = moveDirection.normalized;
        float rotateAmount = Vector3.Cross(moveDirection, transform.up).z;
        rb.angularVelocity = rotateAmount * 400f;
    }
}
