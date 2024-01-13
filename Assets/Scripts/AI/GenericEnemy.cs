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
    [SerializeField] Transform unitTransform;

    //Temporary Testing
    [SerializeField] GameObject destinations;
    
    void Awake(){
        destinations = FindObjectWithTag();
    }

    void Update(){
        float distance = Vector3.Distance(unitTransform, destinations[0]);
        int closestEnemy = 0;
        for(int i = 0; destinations.Length > 0; i++){
            if(distance > Vector3.Distance(unitTransform, destinations[i])){
                distance = Vector3.Distance(unitTransform, destinations[i]);
                closestEnemy = i;
            }
        }

        MoveTo(destinationTransform.position);
        /*if(Vector3.Distance(this.transform.position, friendly.transform.position) > Weapon.range){
            Weapon.Attack();
        }*/
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
