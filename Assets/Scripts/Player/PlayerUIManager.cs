using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager Instance { get; private set; }

    public Image[] bloodEffects;
    public Slider healthBar;
    public Image healthBarSprite;

    void Awake() {
        Instance = this;
    }
}
