using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;                     // To keep track of one GameManager
    //[SerializeField] float playerHealth;                    // Player's current health
    List<GameObject> zombiesInTheLevel;                     // Keeps track of the zombies in the level. Allows for wave start/end

    bool isWaveActive = false;
    

    // This allows us to keep the GameManager script when scenes are reloading or changing
    private void Awake()
    {
        zombiesInTheLevel = new List<GameObject>();

        // Instance of this object already around?
        if (Instance != null)
        {
            // Not anymore dude...
            Destroy(gameObject);    // Destroy this object, another exists. I cry.
            return;
        }

        // This is the first, congrats, you live
        Instance = this;
        DontDestroyOnLoad(gameObject);      // Don't destroy you, you lucky
    }

    public void AddZombieToList(GameObject zombie)
    {
        zombiesInTheLevel.Add(zombie);

        if (!isWaveActive)
        {
            isWaveActive = true; // Wave has started. This is the first zombie in the wave
            Debug.Log("Wave has started");
        }
    }

    public void RemoveZombieFromList(GameObject zombie)
    {
        Debug.Log("Remove Zombie called");
        zombiesInTheLevel.Remove(zombie);
    }

    public bool IsWaveOver()
    {
        Debug.Log("Number of zombers left: " + zombiesInTheLevel.Count);
        if(zombiesInTheLevel.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
