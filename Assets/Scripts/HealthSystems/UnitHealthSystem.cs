using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealthSystem : HealthSystem
{
    //Health Functionality
    [SerializeField] private Transform bar;

    //Visual Elements
    [SerializeField] private SpriteRenderer[] sprites; //Holds Health and Health Background
    [SerializeField] private Color[] displayColor; //Holds Health and Health Background
    [SerializeField] private float flashingDuration;
    [SerializeField] private float flashCycles;
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
        GameManager.Instance.RemoveZombieFromList(gameObject);
        Destroy(gameObject);
    }

    private void SetSize(float sizeNormalized) {
        bar.localScale = new Vector3(sizeNormalized, 1f);
    }

    IEnumerator HealthFlashing(){
        /*float elapsed = 0f;
        for(int i = 0; i >= ; i++){
            while(elapsed <= flashingDuration){
                elapsed += Time.deltaTime;
                yield return null;
            }
        }*/
        
        yield break;
    }

    IEnumerator HideHealth(){
        for(int i = 0; sprites.Length > i; i++){
            sprites[i].color = new Color(displayColor[i].r, displayColor[i].g, displayColor[i].b, 1f);
        }
        
        float elapsed = 0f;
        while(elapsed <= waitBeforeFade){
            elapsed += Time.deltaTime;
            yield return null;
        }
        elapsed = 0f;
        while(elapsed <= fadeDuration){
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, (elapsed / fadeDuration));
            for(int i = 0; sprites.Length > i; i++){
                sprites[i].color = new Color(displayColor[i].r, displayColor[i].g, displayColor[i].b, alpha);
            }
            
            yield return null;
        }
        yield break;
    }
}
