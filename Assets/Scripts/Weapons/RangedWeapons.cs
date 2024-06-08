using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Users;

public class RangedWeapons : WeaponsClass
{
    [SerializeField] private float projectileSpeed;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float recoilTime; // the amount of time it takes between each shot - so the recoil of the weapon
    [SerializeField] private int clipSize; 
    public int ammo; // the total amount of ammo this zombie/player has which is set at the beginning of the wave
    private int counter = 0;
    private bool reloaded = true;

    private float elapsed = 0;
    public int damage;

    private float elapsedTime;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GetClosestTargets targetFinder;
    private Transform target;
    [SerializeField] SpriteRenderer sprite;

    void Start()
    {
        // get Ammo from wherever we're getting that
        ammo = 100; // for testing
        //clipSize = 25; // for testing
    }

    void Update()
    {
        transform.position = characterPos.transform.position;
        Vector2 moveDirection;

        if(characterPos.gameObject.tag == "Player"){
            Vector2 mouseWorldPos = (Vector2)Camera.main.ScreenToWorldPoint((Vector2) Input.mousePosition) - (Vector2)characterPos.position;

            moveDirection = mouseWorldPos;
        }

        else{
            target = targetFinder.GetClosest();
            moveDirection = (Vector2)target.position - (Vector2)characterPos.position;
            //Debug.DrawRay(transform.position, targetWorldPos, Color.red, 0.01f);
        
        }

        moveDirection = moveDirection.normalized;
        float rotateAmount = Vector3.Cross(moveDirection, transform.up).z;
        rb.angularVelocity = -(rotateAmount * 400f);

        elapsedTime += Time.deltaTime;

        if(transform.eulerAngles.z > 180){
            sprite.flipY = false;
        }
        else{
            sprite.flipY = true;
        }

        elapsed -= Time.deltaTime;

    }

    //Called from player controller
    public override void Attack()
    {
        if (ammo > 0 && reloaded && 0 >= elapsed) // NEED TO CHANGE LATER BC THIS WILL AFFECT EVERYONE WITH THIS SCRIPT!! ONLY FOR TESTING
        {    
            if (counter == clipSize){
                Reload();
                counter = 0;
            } 
            else{
                elapsed = recoilTime;
                // get the mouse position
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // current position to mouse position
                directionOfAttack = (mousePos - characterPos.position).normalized;

                // create the projectile 
                GameObject newProjectile = Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);
                // shoot the projectile
                newProjectile.GetComponent<ProjectileMovement>().InitiateMovement(directionOfAttack, projectileSpeed, damage);
                
                //Debug.Log("RangedWeapons Attack() function used.");

                // decreasing ammo and increasing amount of ammo used
                counter += 1;
            }
        }
    }

    // after a certain amount of ammo is used this function is called, and if there is no ammo left - you are done
    // variables used from base(Parent): ReloadSpeed
    public override void Reload()
    {
        StartCoroutine(Reloading());

        //Debug.Log("RangedWeapons Reload() function used.");
    }

    IEnumerator Reloading()
    {
        reloaded = false; 
        yield return new WaitForSeconds(reloadSpeed);
        ammo -= clipSize;
        reloaded = true; 
    }
}
