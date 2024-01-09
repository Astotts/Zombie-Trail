using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemy : MonoBehaviour
{
    //[Serialized] Health (stetson's health manager)
    
    //List<Friendly> //friendly units (Includes player, bus, and companions)

    //Weapon (Malcolm's Weapon)
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float moveSpeed;

    //Temporary Testing
    [SerializeField] Transform destinationTransform;
    
    void Awake(){
        //Setup health
        //Populate the list with all friendly units
    }

    void Update(){
        /*for(amount of friendlies){
            friendly closest = Which is closest?
        }*/

        MoveTo(destinationTransform.position);
        /*if(Vector3.Distance(this.transform.position, friendly.transform.position) > Weapon.range){
            Weapon.Attack();
        }*/
    }

    void MoveTo(Vector3 position){
        Vector3 moveDirection = this.transform.position - position;
        RotateTowards((Vector2)moveDirection);
        //ToDo: Feature to speed up and slow down
        this.transform.position = (Vector2)transform.up * moveSpeed * Time.deltaTime + (Vector2)this.transform.position;
        rb.velocity = Vector2.zero;
    }

    void RotateTowards(Vector2 moveDirection){
        moveDirection = moveDirection.normalized;
        float rotateAmount = Vector3.Cross(moveDirection, transform.up).z;
        rb.angularVelocity = rotateAmount * 400f;
    }
}
