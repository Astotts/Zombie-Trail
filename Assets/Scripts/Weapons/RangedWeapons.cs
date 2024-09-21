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
    public bool reloading = true;

    private float elapsed = 0;
    public int damage;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] GetClosestTargets targetFinder;
    private Transform target;
    [SerializeField] SpriteRenderer sprite;

    //Reload UI 
    [SerializeField] Image[] sprites;
    [SerializeField] float fadeDuration;

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
        if (ammo == 0){
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

    public override void Reload()
    {
        if(!reloading){
            reloading = true;
            StartCoroutine(Reloading());
        }
    }

    IEnumerator Reloading()
    {
        SetOpacity(1);
        reloadSlider.value = 0;
        float reloadElapsed = 0;
        while (reloadElapsed < reloadSpeed){
            reloadElapsed += Time.deltaTime;
            reloadSlider.value = (reloadElapsed / reloadSpeed);
            //Debug.Log(reloadSlider.value);
            yield return null;
        }
        ammo = clipSize;
        for(int i = 0; clipSize > i; i++){
            bulletUI[i].SetActive(true);
        }
        reloading = false;
        StartCoroutine(HideReloadStatus());
    }

    IEnumerator HideReloadStatus(){
        float statusElapsed = 0f;
        while(statusElapsed <= fadeDuration){
            statusElapsed += Time.deltaTime;
            SetOpacity(Mathf.Lerp(1f, 0f, (statusElapsed / fadeDuration)));
            yield return null;
        }
        yield break;
    }

    public void SetOpacity(float a){
        //Debug.Log("Setting Opacity to " + a);
        for(int i = 0; i < sprites.Length; i++){
            sprites[i].color = new Color(1f, 1f, 1f, a);
        }
    }
}
