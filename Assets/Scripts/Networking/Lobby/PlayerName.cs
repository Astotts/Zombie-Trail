using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName : MonoBehaviour
{
    public static PlayerName Instance { get; private set; }
    [SerializeField] TMP_InputField input;
    [SerializeField] Button confirm;
    public string playerName;

    void Awake() {
        Instance = this;
    }
    
    void Start() {
        playerName = "Firefly " + Random.Range(0, 100);
        input.text = playerName;
        confirm.onClick.AddListener(SetPlayerName);
    }

    void SetPlayerName() {
        playerName = input.text;
        LobbyManager.Instance.UpdatePlayerName(playerName);
    }
}
