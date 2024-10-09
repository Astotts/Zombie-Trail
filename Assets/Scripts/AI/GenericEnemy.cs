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

    void Update()
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

    void MoveTo(Vector3 position)
    {
        Vector3 moveDirection = unitRB.transform.position - position;
        RotateTowards(moveDirection);
        //ToDo: Feature to speed up and slow down
        Vector3 newPos = (Vector2)transform.up * moveSpeed * Time.deltaTime + (Vector2)unitRB.position;
        unitRB.MovePosition(newPos);
    }

    void RotateTowards(Vector2 moveDirection)
    {
        moveDirection = moveDirection.normalized;
        float rotateAmount = Vector3.Cross(moveDirection, transform.up).z;
        rb.angularVelocity = rotateAmount * 400f;
    }
}
