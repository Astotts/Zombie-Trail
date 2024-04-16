using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] GameObject[] spawnPoints;
    [SerializeField] GameObject[] zombiePrefabs;

    [SerializeField] public GameObject[] zombies; // used to track the number of zombies 

    public int difficulity = 0; // depending on the difficulity number, there will be either more or less zombs
    public float spawnRate = 0.5f; // the spawn rate of the zombies

    public bool readyForNextWave = false;  

    // Start is called before the first frame update
    void Start()
    {
        zombies = new GameObject[50];
    }

    void OnEnable()
    {
        GameManager.OnStateChange += StartNewWave;
    }

    void OnDisable()
    {
        GameManager.OnStateChange -= StartNewWave;
    }

    public void StartNewWave(GameState state)
    {
        if (state != GameState.WaitEnd)
            return;

        GameManager.StateUpdate(GameState.WaveStart);
        difficulity += 0;      // Update this later, testings

        StartCoroutine(SpawnWave(difficulity));     // Start wave
        readyForNextWave = false;
    }

    IEnumerator SpawnWave(int numOfZombs)
    {
        for (int i = 0; i < numOfZombs; i++)
        {
            int randIndex = GenerateRandomIndex(zombiePrefabs);
            GameObject newZomb = Instantiate(zombiePrefabs[randIndex], GenerateRandomPos(), zombiePrefabs[randIndex].transform.rotation);
            GameManager.Instance.AddZombieToList(newZomb);
            newZomb.GetComponent<NetworkObject>().Spawn();
            //zombies[i] = newZomb; 
            yield return new WaitForSeconds(spawnRate);
        }
        readyForNextWave = true;
        yield break;
    }
    
    // generates a random position at a random spawn point
    private Vector2 GenerateRandomPos()
    {
        int randomSpawnPointIndex = GenerateRandomIndex(spawnPoints);
        float sizeOfSpawn = spawnPoints[randomSpawnPointIndex].GetComponent<BoxCollider2D>().size.y; // only need y bc that's the long portion
        //on both the left/right and upper/lower spawn points (since I just rotated the spawn point)

        // initializing lowermost/leftmost and uppermost/rightmost bounds for spawning 
        float lowerBound;
        float upperBound;
        
        // initializing random x and y positions. 
        float randomPosX = 0;
        float randomPosY = 0; 

        if (randomSpawnPointIndex % 2 == 0) // index 0 is left, index 2 is right
        { // if left or right, x value will be the same, just need a random y value
            lowerBound = spawnPoints[randomSpawnPointIndex].transform.position.y - (sizeOfSpawn/2);
            upperBound = spawnPoints[randomSpawnPointIndex].transform.position.y + (sizeOfSpawn / 2);

            randomPosX = spawnPoints[randomSpawnPointIndex].transform.position.x;
            randomPosY = Random.Range(lowerBound, upperBound);
        }
        else // non even indexes, so 1(lower) and 3(upper)
        { // only need a random x value
            lowerBound = spawnPoints[randomSpawnPointIndex].transform.position.x - (sizeOfSpawn / 2);
            upperBound = spawnPoints[randomSpawnPointIndex].transform.position.x + (sizeOfSpawn / 2);

            randomPosX = Random.Range(lowerBound, upperBound);
            randomPosY = spawnPoints[randomSpawnPointIndex].transform.position.y; 
        }

        Vector2 randomPos = new Vector2(randomPosX, randomPosY);

        return randomPos;
    }

    // generates a random index for an array of game objects
    private int GenerateRandomIndex(GameObject[] arr)
    {
        return Random.Range(0, arr.Length);
    }
}
