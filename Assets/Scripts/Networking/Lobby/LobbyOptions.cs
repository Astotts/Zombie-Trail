using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyOptions : MonoBehaviour
{
    public static LobbyOptions Instance { get; private set; }
    public Button createTabButton;
    public Button findTabButton;

    void Awake() {
        Instance = this;
    }

    void Start() {
        LobbyManager.Instance.OnJoinedLobby += OnJoinedLobby;
        LobbyManager.Instance.OnLeftLobby += OnLeftLobby;
        LobbyManager.Instance.OnKickedFromLobby += OnLeftLobby;
        Hide();
    }

    void OnLeftLobby(object sender, EventArgs e) {
        Show();
    }

    void OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e) {
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
