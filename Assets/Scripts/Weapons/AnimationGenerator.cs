using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class AnimationClassType{
    public enum AnimationType{
        Swing,
        Stab,
        Pound
    }
}

public class AnimationGenerator : MonoBehaviour
{
    [SerializeField] public AnimationClassType.AnimationType type; 

    [Range (0, 5)]
    [SerializeField] public float primaryAnimationTime;
    [Range (0, 5)]
    [SerializeField] public float secondaryAnimationTime;
    [Range (0, 5)]
    [SerializeField] public float tertiaryAnimationTime;

    public virtual void StopAnimating(){
        
    }
}
