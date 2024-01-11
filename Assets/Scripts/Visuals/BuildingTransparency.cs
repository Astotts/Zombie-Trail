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
            Debug.Log("Enter");
            elapsedTime = timeToFade;  
            StartCoroutine("Revert");
            StopCoroutine("Apply");
        }
        
    }

    void OnTriggerExit2D(Collider2D col){
        if(col.tag == "Player"){
            Debug.Log("Exit");
            elapsedTime = timeToFade;
            StartCoroutine("Apply");
            StopCoroutine("Revert"); 
        }
    }

    IEnumerator Revert(){
        float startingAlpha = spriteRenderer.color.a;
        while(0 <= elapsedTime){
            elapsedTime -= Time.deltaTime;
            float scalar = elapsedTime / timeToFade;
            Color temp = spriteRenderer.color;
            temp.a = Mathf.Lerp(0.25f, 1f, scalar);
            spriteRenderer.color = temp;
            yield return null;
        }
        yield break;
    }

    IEnumerator Apply(){
        float startingAlpha = spriteRenderer.color.a;
        while(0 <= elapsedTime){
            elapsedTime -= Time.deltaTime;
            float scalar = elapsedTime / timeToFade;
            Color temp = spriteRenderer.color;
            temp.a = Mathf.Lerp(1f, 0.25f, scalar);
            spriteRenderer.color = temp;
            yield return null;
        }
        yield break;
    }
}
