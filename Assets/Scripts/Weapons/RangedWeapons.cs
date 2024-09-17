using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RangedWeapons : WeaponsClass
{
    [SerializeField] private float projectileSpeed;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float recoilTime; // the amount of time it takes between each shot - so the recoil of the weapon
    [SerializeField] private int clipSize;
    [SerializeField] private GameObject ammoPanel;
    private List<GameObject> bulletUI;
    [SerializeField] private Slider reloadSlider;
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
        bulletUI = new List<GameObject>();
        int children = ammoPanel.transform.childCount;
        for(int i = 0; i < children; i++){
            bulletUI.Add(ammoPanel.transform.GetChild(i).gameObject);
        }
    }

    void Update()
    {
        transform.position = characterPos.position;
        Vector2 moveDirection;

        if(characterPos.gameObject.tag == "Player"){
            Vector2 mouseWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)characterPos.position;

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
        if (ammo >= 0 && reloaded) // NEED TO CHANGE LATER BC THIS WILL AFFECT EVERYONE WITH THIS SCRIPT!! ONLY FOR TESTING
        {    
            if (ammo == 0){
                Debug.Log("Calling first reload");
                reloaded = false;
                Reload();
            } 
            else if(0 >= elapsed){
                elapsed = recoilTime;
                // get the mouse position
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // current position to mouse position
                directionOfAttack = (mousePos - characterPos.position).normalized;

                // create & shoot the projectile 
                GameObject newProjectile = Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);
                newProjectile.GetComponent<ProjectileMovement>().InitiateMovement(directionOfAttack, projectileSpeed, damage);
                if(ammo > 0){
                    bulletUI[ammo - 1].SetActive(false);
                }
                ammo -= 1;
            }
        }
        else if(reloaded){
            reloaded = false;
            Reload();
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
        reloadSlider.value = 0;
        float elapsed = 0;
        while (elapsed < reloadSpeed){
            elapsed += Time.deltaTime;
            reloadSlider.value = (elapsed / reloadSpeed);
            Debug.Log(reloadSlider.value);
            yield return null;
        }
        ammo = clipSize;
        reloaded = true; 
        for(int i = 0; clipSize > i; i++){
            bulletUI[i].SetActive(true);
        }
    }
}
