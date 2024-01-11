using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapons : WeaponsClass
{
    [SerializeField] ContactFilter2D filter;
    private float elapsedTime;
    [SerializeField] private Collider2D attackHitBox;
    private Collider2D[] enemies;
    [SerializeField] Rigidbody2D rb;

    void Awake(){
        enemies = new Collider2D[10];
    }

    void Update()
    {

        attackHitBox.transform.position = characterPos.position;

        Vector2 mouseWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)characterPos.position;

        Vector2 moveDirection = mouseWorldPos;

        Debug.DrawRay(characterPos.position, mouseWorldPos, Color.red, 0.5f);

        moveDirection = moveDirection.normalized;
        float rotateAmount = Vector3.Cross(moveDirection, attackHitBox.transform.up).z;
        rb.angularVelocity = -(rotateAmount * 400f);

        elapsedTime += Time.deltaTime;
    }

    // variables used from base(Parent): RangeOfAttack and DirectionOfAttack
    public override void Attack()
    {   
        if (Input.GetMouseButtonDown(0) && elapsedTime > reloadSpeed) // NEED TO CHANGE LATER BC THIS WILL AFFECT EVERYONE WITH THIS SCRIPT!! ONLY FOR TESTING
        { 
            elapsedTime = 0;
            attackHitBox.OverlapCollider(filter, enemies);

            foreach(Collider2D collider in enemies){
                if(collider != null && collider.tag == "Enemy"){
                    //Take Damage   
                }
            }

            Debug.Log("MeleeWeapons Attack() function used.");
        }
    }

    // after each attack this function is called in order to serve as a cooldown between attacks
    // variables used from base(Parent): ReloadSpeed
    public override void Reload()
    {

        Debug.Log("MeleeWeapons Reload() function used.");
    }
}
