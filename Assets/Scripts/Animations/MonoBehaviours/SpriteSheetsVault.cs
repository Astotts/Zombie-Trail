using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetsVault : MonoBehaviour
{
    private static readonly Dictionary<int, Sprite> spriteMap = new();

    [SerializeField] private List<AnimationControllerSO> animationControllers;

    void Awake()
    {
        foreach (AnimationControllerSO controllerSO in animationControllers)
        {
            foreach (AnimationClip clip in controllerSO.AnimationClips)
            {
                foreach (Sprite sprite in clip.Clip.Sprites)
                {
                    spriteMap[Animator.StringToHash(sprite.name)] = sprite;
                }
            }
        }
    }

    public static bool GetSprite(int instanceId, out Sprite sprite)
    {
        return spriteMap.TryGetValue(instanceId, out sprite);
    }
}
