using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealthSystem : HealthSystem
{
    //Health Functionality
    [SerializeField] private Transform bar;
    [SerializeField] private GameObject unit;

    //Visual Elements
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Color displayColor;
    [SerializeField] private float flashingDuration;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float waitBeforeFade;
    
    public override void Awake()
    {
        currentHealth = maxHealth;
        SetSize(1f); //Size is normalized so 1 is 100% health
    }

    public override void AlterHealth(int amount) {
        currentHealth += amount;
        if(currentHealth > 0){
            SetSize(((float)currentHealth / (float)maxHealth)); //Since health variables are ints must cast to float values
            //for(int i = sprites.Length; i > 0; i--){
                //sprite.color = new Color(displayColor.r, displayColor.g, displayColor.b, displayColor.a);
            //}
            StartCoroutine("HideHealth");
            StartCoroutine("HealthFlashing");
        }
        else {
            SetSize(0f); //Size is normalized so 0 is 0% health
            Die();
        }    
    }

    public override void Die(){
        StopCoroutine("HealthFlashing");
        StopCoroutine("ShowHealth");

        // wave manager helper
        GameObject.Find("Wave Manager").GetComponent<WaveManager>().RemoveZombie();

        Destroy(unit);
    }

    private void SetSize(float sizeNormalized) {
        bar.localScale = new Vector3(sizeNormalized, 1f);
    }

    IEnumerator HealthFlashing(){
        //while(){
            //sprite.color = Color.white;
        //}
        
        yield break;
    }

    IEnumerator HideHealth(){
        /*yield return new WaitForSeconds(waitBeforeFade);
        //while(){}
        */
        yield break;
    }
}
