using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesManager : MonoBehaviour
{
    public static SpritesManager Instance { get; private set; }

    public Sprite ownerIcon;
    public Sprite playerIcon;
    public Sprite john;
    public Sprite jack;
    public Sprite jane;

    void Awake() {
        Instance = this;
    }
}
