using UnityEngine;

[CreateAssetMenu(fileName = "AnimationClip", menuName = "Scriptable Objects/Animation/Clip")]
public class AnimationClipSO : ScriptableObject
{
    [field: SerializeField] public Sprite[] Sprites { get; private set; }
    [field: SerializeField] public float[] Durations { get; private set; }
}
