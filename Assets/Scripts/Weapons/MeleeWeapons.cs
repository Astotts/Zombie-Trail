using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MeleeWeapons : WeaponsClass
{
    [SerializeField] ContactFilter2D filter;
    private float elapsedTime;
    [SerializeField] private Collider2D attackHitBox;
    private Collider2D[] enemies;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GetClosestTargets targetFinder;
    private Transform target;
    [SerializeField] private int damage;

    void Awake(){
        enemies = new Collider2D[10];
    }

    void Update()
    {
        if (!IsOwner) return;
        Vector2 moveDirection;

        attackHitBox.transform.position = characterPos.position;
        rb.transform.position = characterPos.position;

        if(characterPos.gameObject.CompareTag("Player"))
        {
            Vector2 mouseWorldPos = ((Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z)) - (Vector2)characterPos.position) * -1;
            moveDirection = mouseWorldPos;
        }
        else{
            target = targetFinder.GetClosest();
            moveDirection = (Vector2)target.position - (Vector2)characterPos.position;
            //Debug.DrawRay(transform.position, targetWorldPos, Color.red, 0.01f);
        
        }
        

        //Debug.DrawRay(characterPos.position, mouseWorldPos, Color.red, 0.5f);

        moveDirection = moveDirection.normalized;
        float rotateAmount = Vector3.Cross(moveDirection, attackHitBox.transform.up).z;
        rb.angularVelocity = -(rotateAmount * 400f);

        elapsedTime += Time.deltaTime;
    }

    // variables used from base(Parent): RangeOfAttack and DirectionOfAttack
    public override void Attack()
    {   
        if (!IsOwner) return;
        if (elapsedTime > reloadSpeed) // NEED TO CHANGE LATER BC THIS WILL AFFECT EVERYONE WITH THIS SCRIPT!! ONLY FOR TESTING
        {
            elapsedTime = 0;
            attackHitBox.OverlapCollider(filter, enemies);
            
            if(characterPos.gameObject.tag == "Player" && Input.GetMouseButtonDown(0)){
                foreach(Collider2D collider in enemies){
                    if(collider != null && collider.tag == "Enemy"){
                        collider.GetComponent<HealthSystem>().AlterHealth(-damage);   
                    }
                }
            }
            else{
                foreach(Collider2D collider in enemies){
                    if(collider != null && collider.tag == "Player"){
                        collider.GetComponent<HealthSystem>().AlterHealth(-damage);   
                    }
                }
            }

            Array.Clear(enemies, 0, enemies.Length);
            
        }
        
    }

    // after each attack this function is called in order to serve as a cooldown between attacks
    // variables used from base(Parent): ReloadSpeed
    public override void Reload()
    {

        Debug.Log("MeleeWeapons Reload() function used.");
    }


}
