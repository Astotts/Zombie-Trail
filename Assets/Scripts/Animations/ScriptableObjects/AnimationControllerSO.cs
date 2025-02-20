using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationController", menuName = "Scriptable Objects/Animation/Controller")]
public class AnimationControllerSO : ScriptableObject
{
    [field: SerializeField] public AnimationClip[] AnimationClips { get; private set; }
}

[Serializable]
public class AnimationClip
{
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField] public AnimationClipSO Clip { get; private set; }
}