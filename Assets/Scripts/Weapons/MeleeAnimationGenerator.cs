using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class MeleeAnimationGenerator : AnimationGenerator
{
    //Transform and Animation State    

    [Header ("Swing Animation Variables")]
    [SerializeField] public Transform centerPoint;
    [SerializeField] public Transform hingePoint;
    [SerializeField] public Transform aimVector;
    public bool animRunning = false;
    [SerializeField] public bool oneSidedSwing;
    public bool swingSide;
    [Range (-180, 180)]
    [SerializeField] public float offsetDegrees;   
    [Range (-180, 180)]
    [SerializeField] public float centerStartDegrees, centerFinalDegrees;
    [Range (-180, 180)]
    [SerializeField] public float hingeStartDegrees, hingeFinalDegrees;
    [Header ("Stab Animation Variables")]
    [SerializeField] Transform weaponTransform;
    [SerializeField] Transform startingTransform;
    [Range (0, 1)]
    [SerializeField] public float forwardOffset;

void Update(){
    
    if(Input.GetMouseButton(0) && !animRunning){
        animRunning = true;
        switch(type){
            case AnimationClassType.AnimationType.Swing :
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
            break;
            case AnimationClassType.AnimationType.Stab :
                StartCoroutine("StabAnim");  
            break;
            case AnimationClassType.AnimationType.Pound :
                StartCoroutine("PoundAnim");  
            break;
        }
    }
}

public override void StopAnimating(){
    if(!animRunning)
    return;

    animRunning = false;
    switch(type){
        case AnimationClassType.AnimationType.Swing :
            StopCoroutine("RightSwingAnim");
            StopCoroutine("LeftSwingAnim");
            StopCoroutine("SwingAnim");
            centerPoint.localEulerAngles = new Vector3(0, 0, centerStartDegrees);
            hingePoint.localEulerAngles = new Vector3(0, 0, hingeStartDegrees);
        break;
        case AnimationClassType.AnimationType.Stab :
            StopCoroutine("StabAnim");
            weaponTransform.position = startingTransform.position;
        break;
        case AnimationClassType.AnimationType.Pound :
            StopCoroutine("PoundAnim");
            centerPoint.localEulerAngles = new Vector3(0, 0, centerStartDegrees);
            hingePoint.localEulerAngles = new Vector3(0, 0, hingeStartDegrees);
        break;
    }
}
    
    IEnumerator SwingAnim(){
        float elapsed = 0f;
        elapsed += Time.deltaTime;
        while(elapsed <= primaryAnimationTime){
            centerPoint.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(centerStartDegrees, centerFinalDegrees, elapsed / primaryAnimationTime));
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
            centerPoint.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(centerStartDegrees, centerFinalDegrees, elapsed / primaryAnimationTime));
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
            centerPoint.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(centerFinalDegrees, centerStartDegrees, elapsed / primaryAnimationTime));
            hingePoint.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(hingeFinalDegrees, hingeStartDegrees, elapsed / primaryAnimationTime));
            elapsed += Time.deltaTime;
            yield return null;
            //x^2-(x^3/35)
        }
        animRunning = false;
        yield break;
    }

    IEnumerator StabAnim(){
        //Stab
        float elapsed = 0f;
        while(elapsed <= primaryAnimationTime){
            weaponTransform.position = Vector3.Lerp(startingTransform.position, weaponTransform.up * forwardOffset + weaponTransform.position, elapsed / primaryAnimationTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        //Wait
        elapsed = 0f;
        while(elapsed <= secondaryAnimationTime){ 
            elapsed += Time.deltaTime;
            yield return null;
        }
        //Unstab
        elapsed = 0f;
        while(elapsed <= tertiaryAnimationTime){
            weaponTransform.position = Vector3.Lerp(weaponTransform.position, startingTransform.position, elapsed / tertiaryAnimationTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        animRunning = false;
        yield break;
    }

   IEnumerator PoundAnim(){
        float elapsed = 0f;
        elapsed += Time.deltaTime;
        //Wind Up
        while(elapsed <= primaryAnimationTime){
            centerPoint.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(centerStartDegrees, centerFinalDegrees, elapsed / primaryAnimationTime));
            hingePoint.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(hingeStartDegrees, hingeFinalDegrees, elapsed / primaryAnimationTime));
            elapsed += Time.deltaTime;
            yield return null;
        }
        //Smash
        elapsed = 0f;
        while(elapsed <= secondaryAnimationTime){
            elapsed += Time.deltaTime;
            yield return null;
        }
        //Wind Down
        elapsed = 0f;
        while(elapsed <= tertiaryAnimationTime){
            centerPoint.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(centerFinalDegrees, centerStartDegrees, elapsed / tertiaryAnimationTime));
            hingePoint.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(hingeFinalDegrees, hingeStartDegrees - offsetDegrees, elapsed / tertiaryAnimationTime));
            elapsed += Time.deltaTime;
            yield return null;
        }
        animRunning = false;
        yield break;
    }
}


//Custom Editor to Disable non-relevant fields in the editor

/*[CustomEditor(typeof(MeleeAnimationGenerator))]
public class MeleeAnimationGeneratorEditor : Editor
{
  public override void OnInspectorGUI()
  {
    var myScript = target as MeleeAnimationGenerator;

    myScript.type = (AnimationClassType.AnimationType)EditorGUILayout.EnumFlagsField("Animation Type", myScript.type);

    switch(myScript.type){
        case AnimationClassType.AnimationType.Swing :
            myScript.offsetDegrees = EditorGUILayout.Slider("offsetDegrees", myScript.offsetDegrees , -180 , 180);
            myScript.centerStartDegrees = EditorGUILayout.Slider("centerStartDegrees", myScript.centerStartDegrees , -180 , 180);
            myScript.centerFinalDegrees = EditorGUILayout.Slider("centerFinalDegrees", myScript.centerFinalDegrees , -180 , 180);
            myScript.hingeStartDegrees = EditorGUILayout.Slider("hingeStartDegrees", myScript.hingeStartDegrees , -180 , 180);
            myScript.hingeFinalDegrees = EditorGUILayout.Slider("hingeFinalDegrees", myScript.hingeFinalDegrees , -180 , 180);
            myScript.primaryAnimationTime = EditorGUILayout.Slider("primaryAnimationTime", myScript.primaryAnimationTime , 0 , 5);
            myScript.secondaryAnimationTime = EditorGUILayout.Slider("primaryAnimationTime", myScript.primaryAnimationTime , 0 , 5);
        break;
        case AnimationClassType.AnimationType.Stab :
        break;
        case AnimationClassType.AnimationType.Pound :
        break;
    }
    
  }
}*/