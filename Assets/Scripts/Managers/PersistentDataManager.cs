using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager Instance { get; set; }

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There are more than one PersistentDataManager!");
            return;
        }
        Instance = this;
    }

    void SaveGame()
    {

    }
}