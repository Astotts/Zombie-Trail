using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTransparency : MonoBehaviour
{
    [SerializeField]SpriteRenderer spriteRenderer;
    [SerializeField] float timeToFade;
    float elapsedTime;
    
    void Awake(){
        elapsedTime = timeToFade;
    }
    
    void OnTriggerEnter2D(Collider2D col){
        if(col.tag == "Player"){ 
            StartCoroutine("Apply", spriteRenderer.color.a);
            StopCoroutine("Revert"); 
        }
        
    }

    void OnTriggerExit2D(Collider2D col){
        if(col.tag == "Player"){
            StartCoroutine("Revert", spriteRenderer.color.a);
            StopCoroutine("Apply");
        }
    }

    IEnumerator Revert(float startingAlpha){
        while(timeToFade >= elapsedTime){
            elapsedTime += Time.deltaTime;
            float scalar = elapsedTime / timeToFade;
            Color temp = spriteRenderer.color;
            temp.a = Mathf.Lerp(startingAlpha, 1f, scalar);
            spriteRenderer.color = temp;
            yield return null;
        }
        elapsedTime = 2;
        yield break;
    }

    //Makes object transparent over time
    IEnumerator Apply(float startingAlpha){
        while(0 <= elapsedTime){
            elapsedTime -= Time.deltaTime;
            float scalar = elapsedTime / timeToFade;
            Color temp = spriteRenderer.color;
            temp.a = Mathf.Lerp(0.25f, startingAlpha, scalar);
            
            spriteRenderer.color = temp;
            yield return null;
        }
        elapsedTime = 0;
        yield break;
    }
}
