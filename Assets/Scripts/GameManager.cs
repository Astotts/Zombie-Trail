using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;                   // To keep track of one GameManager

    // This allows us to keep the GameManager script when scenes are reloading or changing
    private void Awake()
    {
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
