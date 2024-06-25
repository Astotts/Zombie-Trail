using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyProfileUI : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] TMP_Text lobbyName;
    [SerializeField] TMP_Text isFull;
    [SerializeField] TMP_Text capacity;
    [SerializeField] Button joinLobbyButton;
    [SerializeField] Transform transistion;

    const string FULL = "<color=red>FULL";
    const string OPEN = "<color=green>OPEN";

    bool confirm;
    string lobbyCode;

    public void UpdateInfo(Lobby lobby) {
        lobbyName.text = lobby.Name;

        if (lobby.AvailableSlots == 0)
            isFull.text = FULL;
        else
            isFull.text = OPEN;
        lobbyCode = lobby.LobbyCode;
        capacity.text = lobby.MaxPlayers - lobby.AvailableSlots + "/" + lobby.MaxPlayers;
        joinLobbyButton.onClick.AddListener(() => {
        if (confirm)
            LobbyManager.Instance.JoinLobby(lobby);
        else{
            transistion.gameObject.SetActive(true);
            confirm = true;
        }
        });
    }

    public void OnPointerExit(PointerEventData e)
    {
        transistion.gameObject.SetActive(false);
    }
}
