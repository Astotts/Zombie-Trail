using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public static MenuUI Instance { get; private set; }

    [SerializeField] Button startButton;
    [SerializeField] Button joinButton;
    [SerializeField] TMP_InputField codeInput;

    void Awake() {
        Instance = this;
        startButton.onClick.AddListener(StartWorld);
        joinButton.onClick.AddListener(JoinWorld);
    }

    void StartWorld() {
        RelayManager.Instance.CreateRelay();
    }

    void JoinWorld() {
        string code = codeInput.text;
        RelayManager.Instance.JoinRelay(code);
    }
}
