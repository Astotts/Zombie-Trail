using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemy : MonoBehaviour
{
    [SerializeField] WeaponsClass weapon;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float moveSpeed;
    [SerializeField] Transform unitTransform;
    [SerializeField] GetClosestTargets targetFinder;
    private Transform target;
    void Update(){
        transform.position = unitTransform.position;    
        target = targetFinder.GetClosest();

        MoveTo(target.position);
        //Debug.Log(targetFinder.GetDistance());
        if(targetFinder.GetDistance() < weapon.range){
            Debug.Log("Working");
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
