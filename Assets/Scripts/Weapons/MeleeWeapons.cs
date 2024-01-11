using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapons : WeaponsClass
{
    [SerializeField] float elapsedTime;
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && elapsedTime > 0) // NEED TO CHANGE LATER BC THIS WILL AFFECT EVERYONE WITH THIS SCRIPT!! ONLY FOR TESTING
        {
            Attack(); 
        }
    }

    // variables used from base(Parent): RangeOfAttack and DirectionOfAttack
    public override void Attack()
    {   
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // current position to mouse position
        directionOfAttack = (mousePos - this.transform.position).normalized;



        Debug.Log("MeleeWeapons Attack() function used.");
    }

    // after each attack this function is called in order to serve as a cooldown between attacks
    // variables used from base(Parent): ReloadSpeed
    public override void Reload()
    {

        Debug.Log("MeleeWeapons Reload() function used.");
    }
}
