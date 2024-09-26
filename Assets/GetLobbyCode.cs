using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetLobbyCode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI display;
    [SerializeField] private string lobbyCode;
    public void Awake(){
        StartCoroutine(GetJoinCode());
    }

    private IEnumerator GetJoinCode()
    {
        // Keep searching for the spawn point until it is found
        while (RelayManager.Instance == null || RelayManager.JoinCode == null)
        {
            yield return null;
        }
        
        display.text = RelayManager.JoinCode;
    }
}
