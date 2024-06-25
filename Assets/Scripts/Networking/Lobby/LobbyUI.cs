using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Instance { get; private set; }
    [SerializeField] TMP_Text lobbyName;
    [SerializeField] TMP_Text code;
    [SerializeField] TMP_Text password;
    [SerializeField] Transform playerListContents;
    [SerializeField] Button readyButton;
    [SerializeField] GameObject playerProfilePrefab;

    public Player host;


    bool isReady;
    void Awake() {
        Instance = this;
        readyButton.onClick.AddListener(SetReady);
    }

    void Start() {
        LobbyManager.Instance.OnJoinedLobby += OnLobbyJoined;
        LobbyManager.Instance.OnJoinedLobby += OnLobbyUpdate;
        LobbyManager.Instance.OnJoinedLobbyUpdate += OnLobbyUpdate;
        LobbyManager.Instance.OnLeftLobby += OnLobbyLeft;
        LobbyManager.Instance.OnKickedFromLobby += OnLobbyLeft;
        Hide();
    }

    void SetReady() {
        isReady = !isReady;
        LobbyManager.Instance.UpdatePlayerReady(isReady);
    }

    void OnLobbyLeft(object sender, EventArgs e) {
        ClearPlayer();
        Hide();
    }

    void OnLobbyJoined(object sender, LobbyManager.LobbyEventArgs e) {
        Show();
    }

    void OnLobbyUpdate(object sender, LobbyManager.LobbyEventArgs e) {
        UpdateLobby(LobbyManager.Instance.GetJoinedLobby());
    }

    void UpdateLobby(Lobby joinedLobby) {
        lobbyName.text = joinedLobby.Name;
        code.text = "Lobby Code: " + joinedLobby.LobbyCode;

        ClearPlayer();

        foreach (Player player in joinedLobby.Players) {
            GameObject go = Instantiate(playerProfilePrefab, playerListContents);
            PlayerProfileUI ui = go.GetComponent<PlayerProfileUI>();

            ui.SetKickButtonVisible(
                LobbyManager.Instance.IsLobbyHost() &&
                player.Id != AuthenticationService.Instance.PlayerId
            );

            ui.SetOwnerIcon(player == joinedLobby.Players[0]);

            ui.UpdateProfile(player);
        }
    }

    void ClearPlayer() {
        foreach (Transform child in playerListContents)
            Destroy(child.gameObject);
    }

    void Show() {
        gameObject.SetActive(true);
    }

    void Hide() {
        gameObject.SetActive(false);
    }
}
