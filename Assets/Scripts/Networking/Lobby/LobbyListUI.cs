using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListUI : MonoBehaviour
{
    public static LobbyListUI Instance { get; private set; }

    [SerializeField] Button tab;
    [SerializeField] Transform contents;
    [SerializeField] TMP_InputField lobbyCodeInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] Button refreshButton;
    [SerializeField] Button joinButton;


    [SerializeField] GameObject lobbyProfilePrefab;
    void Awake() {
        Instance = this;
        tab.onClick.AddListener(TabButtonClicked);
        refreshButton.onClick.AddListener(RefreshButtonClicked);
        joinButton.onClick.AddListener(JoinCodeButtonClicked);
    }

    void Start() {
        LobbyManager.Instance.OnLobbyListChanged += UpdateLobbyList;
        LobbyManager.Instance.OnJoinedLobby += JoinedLobby;
        LobbyManager.Instance.OnLeftLobby += LeaveLobby;
        LobbyManager.Instance.OnKickedFromLobby += LeaveLobby;
    }

    void TabButtonClicked() {
        Show();
        LobbyCreateUI.Instance.Hide();
    }

    void JoinedLobby(object sender, LobbyManager.LobbyEventArgs e) {
        Hide();
    }

    void LeaveLobby(object sender, EventArgs e) {
        Show();
    }

    void UpdateLobbyList(object sender, LobbyManager.OnLobbyListChangedEventArgs e) {
        UpdateLobbyList(e.lobbyList);
    }

    void RefreshButtonClicked() {
        LobbyManager.Instance.RefreshLobbyList();
    }

    void JoinCodeButtonClicked() {
        string code = lobbyCodeInput.text;
        // string password = passwordInput.text;

        LobbyManager.Instance.JoinLobbyByCode(code);
    }

    void UpdateLobbyList(List<Lobby> lobbies) {
        foreach (Transform child in contents) {
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbies) {
            GameObject go = Instantiate(lobbyProfilePrefab, contents);
            LobbyProfileUI ui = go.GetComponent<LobbyProfileUI>();
            ui.UpdateInfo(lobby);
        }
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
    public void Show() {
        gameObject.SetActive(true);
    }
}
