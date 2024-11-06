using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetRelayCode : MonoBehaviour
{
    [SerializeField] TMP_Text ui;
    void Start()
    {
        ui.text = RelayManager.JoinCode;
    }
}
