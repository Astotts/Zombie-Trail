using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    public static LobbyCreateUI Instance { get; private set; }
    [SerializeField] Button tab;
    [SerializeField] TMP_InputField lobbyNameText;
    [SerializeField] TMP_InputField passwordText;
    [SerializeField] Toggle isPrivateToggle;
    [SerializeField] Button confirmButton;

    void Awake() {
        Instance = this;
        tab.onClick.AddListener(TabButtonClicked);
        confirmButton.onClick.AddListener(CreateLobby);
        Hide();
    }

    void CreateLobby() {
        LobbyOptions.Instance.createTabButton.onClick.AddListener(Show);
        LobbyOptions.Instance.findTabButton.onClick.AddListener(Hide);
        string name = lobbyNameText.text;
        string pass = passwordText.text;
        bool isPrivate = isPrivateToggle.isOn;

        LobbyManager.Instance.CreateLobby(name, LobbyManager.MAX_PLAYER, isPrivate);
        Hide();
    }

    void TabButtonClicked() {
        Show();
        LobbyListUI.Instance.Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
