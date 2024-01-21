using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemy : MonoBehaviour
{

    //Unit Movement
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float moveSpeed;
    [SerializeField] Transform unitTransform;

    //Tracking Destination
    [SerializeField] GetClosestTargets targetFinder;
    private Transform target;
    [SerializeField] WeaponsClass weapon;

    //Animation
    [SerializeField] Animator animator;
    private Vector2 lastPos;

    void Update(){
        transform.position = unitTransform.position;    
        target = targetFinder.GetClosest();

        //Animator Variables
        Vector2 distTraveled = (Vector2)transform.position - lastPos;
        lastPos = (Vector2)transform.position;
        animator.SetFloat("X", distTraveled.x);
        animator.SetFloat("Y", distTraveled.y);

        MoveTo(target.position);
        //Debug.Log(targetFinder.GetDistance());
        if(targetFinder.GetDistance() < weapon.range){
            weapon.Attack();
        }
    }

    void MoveTo(Vector3 position){
        Vector3 moveDirection = unitTransform.position - position;
        RotateTowards((Vector2)moveDirection);
        //ToDo: Feature to speed up and slow down
        unitTransform.position = (Vector2)transform.up * moveSpeed * Time.deltaTime + (Vector2)this.transform.position;
        rb.velocity = Vector2.zero;
    }

    void RotateTowards(Vector2 moveDirection){
        moveDirection = moveDirection.normalized;
        float rotateAmount = Vector3.Cross(moveDirection, transform.up).z;
        rb.angularVelocity = rotateAmount * 400f;
    }
}
