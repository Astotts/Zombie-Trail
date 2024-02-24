using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SwingAnimation : MonoBehaviour
{
    //Transform and Animation State    
    [SerializeField] private Transform centerPoint;
    [SerializeField] private Transform hingePoint;
    [SerializeField] private Transform aimVector;
    private bool animRunning = false;
    [SerializeField] private bool oneSidedSwing;
    private bool swingSide;
    
    
    [SerializeField] private float offsetDegrees;   
    [SerializeField] private float centerStartDegrees, centerFinalDegrees;
    [SerializeField] private float hingeStartDegrees, hingeFinalDegrees;
    [SerializeField] private float primaryAnimationTime;
    [SerializeField] private float secondaryAnimationTime;

    


void Update(){
    if(Input.GetMouseButton(0) && !animRunning){
        animRunning = true;
        if(oneSidedSwing){
            if(swingSide){
                swingSide = false;
                StartCoroutine("LeftSwingAnim");  
            }
            else{
                swingSide = true;
                StartCoroutine("RightSwingAnim");  
            }
        }
        else{
            StartCoroutine("SwingAnim");  
        }
        
    }
}
    
    IEnumerator SwingAnim(){
        float elapsed = 0f;
        elapsed += Time.deltaTime;
        while(elapsed <= primaryAnimationTime){
            centerPoint.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(centerStartDegrees, centerFinalDegrees, (8 * elapsed * elapsed) / primaryAnimationTime));
            hingePoint.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(hingeStartDegrees, hingeFinalDegrees, elapsed / primaryAnimationTime));
            elapsed += Time.deltaTime;
            yield return null;
            //x^2-(x^3/35)
        }
        elapsed = 0f;
        while(elapsed <= secondaryAnimationTime){
            centerPoint.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(centerFinalDegrees, centerStartDegrees, elapsed / secondaryAnimationTime));
            hingePoint.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(hingeFinalDegrees, hingeStartDegrees - offsetDegrees, elapsed / secondaryAnimationTime));
            elapsed += Time.deltaTime;
            yield return null;
        }
        animRunning = false;
        yield break;
    }

    IEnumerator RightSwingAnim(){
        float elapsed = 0f;
        elapsed += Time.deltaTime;
        while(elapsed <= primaryAnimationTime){
            centerPoint.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(centerStartDegrees, centerFinalDegrees, (8 * elapsed * elapsed) / primaryAnimationTime));
            hingePoint.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(hingeStartDegrees, hingeFinalDegrees, elapsed / primaryAnimationTime));
            elapsed += Time.deltaTime;
            yield return null;
            //x^2-(x^3/35)
        }
        animRunning = false;
        yield break;
    }

    IEnumerator LeftSwingAnim(){
        float elapsed = 0f;
        elapsed += Time.deltaTime;
        while(elapsed <= primaryAnimationTime){
            centerPoint.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(centerFinalDegrees, centerStartDegrees, (8 * elapsed * elapsed) / primaryAnimationTime));
            hingePoint.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(hingeFinalDegrees, hingeStartDegrees, elapsed / primaryAnimationTime));
            elapsed += Time.deltaTime;
            yield return null;
            //x^2-(x^3/35)
        }
        animRunning = false;
        yield break;
    }
}