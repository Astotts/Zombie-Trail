using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapons : WeaponsClass
{
    [SerializeField] private float projectileSpeed;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float RecoilTime; // the amount of time it takes between each shot - so the recoil of the weapon
    [SerializeField] private int ClipSize; // IF WE WANT, so after you shoot a certain amount of times the Reload() function
    // will be called or we will scratch this and only use reload as a cooldown or the recoil time between attacks like it is with melee
    public int Ammo; // the total amount of ammo this zombie/player has which is set at the beginning of the wave

    void Start()
    {
        // get Ammo from wherever we're getting that
        Ammo = 100; // for testing
        ClipSize = 25; // for testing
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // NEED TO CHANGE LATER BC THIS WILL AFFECT EVERYONE WITH THIS SCRIPT!! ONLY FOR TESTING
        {
            Attack(); 
        }
    }

    // variables used from base(Parent): RangeOfAttack and DirectionOfAttack
    public override void Attack()
    {
        if (Ammo%ClipSize == 0)
        {
            Reload();
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        DirectionOfAttack = (mousePos - this.transform.position).normalized; 

        GameObject newProjectile = Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);
        Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();

        if(rb != null)
        {
            rb.velocity = DirectionOfAttack * projectileSpeed;
        }

        Debug.Log("RangedWeapons Attack() function used.");
    }

    // after a certain amount of ammo is used this function is called, and if there is no ammo left - you are done
    // variables used from base(Parent): ReloadSpeed
    public override void Reload()
    {
        

        Debug.Log("RangedWeapons Reload() function used.");
    }
}
