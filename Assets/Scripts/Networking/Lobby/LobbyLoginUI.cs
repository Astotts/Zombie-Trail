using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyLoginUI : MonoBehaviour
{
    public static LobbyLoginUI Instance { get; private set; }

    [SerializeField] Button loginButton;

    void Awake() {
        Instance = this;
    }

    void Start() {
        loginButton.onClick.AddListener(Login);
    }

    void Login() {
        LobbyManager.Instance.Authenticate(PlayerName.Instance.playerName);
        gameObject.SetActive(false);
        LobbyOptions.Instance.gameObject.SetActive(true);
    }
    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
