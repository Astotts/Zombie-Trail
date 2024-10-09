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
    // public override void OnNetworkSpawn()
    // {
    //     if (!IsOwner)
    //     {
    //         enabled = false;
    //         return;
    //     }
    //     base.OnNetworkSpawn();
    // }

    void Awake()
    {
        enemies = new Collider2D[10];
    }

    void Update()
    {
        Vector2 moveDirection;

        attackHitBox.transform.position = characterPos.position;
        rb.transform.position = characterPos.position;

        if (characterPos.gameObject.CompareTag("Player"))
        {
            Vector2 mouseWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)characterPos.position;

            moveDirection = mouseWorldPos;
        }
        else
        {
            target = targetFinder.GetClosest();
            moveDirection = (Vector2)target.position - (Vector2)characterPos.position;
            //Debug.DrawRay(transform.position, targetWorldPos, Color.red, 0.01f);

        }


        //Debug.DrawRay(characterPos.position, mouseWorldPos, Color.red, 0.5f);

        moveDirection = moveDirection.normalized;
        // float rotateAmount = Vector3.Cross(moveDirection, attackHitBox.transform.up).z;
        // rb.angularVelocity = -(rotateAmount * 400f);
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 2 * Time.deltaTime);

        elapsedTime += Time.deltaTime;
    }

    // variables used from base(Parent): RangeOfAttack and DirectionOfAttack
    public override void Attack()
    {
        if (elapsedTime > reloadSpeed) // NEED TO CHANGE LATER BC THIS WILL AFFECT EVERYONE WITH THIS SCRIPT!! ONLY FOR TESTING
        {
            elapsedTime = 0;
            attackHitBox.OverlapCollider(filter, enemies);

            if (characterPos.CompareTag("Player") && Input.GetMouseButtonDown(0))
            {
                foreach (Collider2D collider in enemies)
                {
                    if (collider != null && collider.CompareTag("Enemy"))
                    {
                        collider.GetComponent<IDamageable>().Damage(damage);
                    }
                }
            }
            else
            {
                foreach (Collider2D collider in enemies)
                {
                    if (collider != null && collider.CompareTag("Player"))
                    {
                        collider.GetComponent<IDamageable>().Damage(damage);
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

    }


}
