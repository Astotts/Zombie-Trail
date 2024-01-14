using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetClosestTargets : MonoBehaviour
{
    
    [SerializeField] List<GameObject> destinations;
    [SerializeField] string[] tagToFind;
    private GameObject target;
    // Start is called before the first frame update
    void Awake()
    {
        destinations = new List<GameObject>();
        for(int i = 0; tagToFind.Length > i; i++){
            destinations.AddRange(GameObject.FindGameObjectsWithTag(tagToFind[i]));
        }
        
        int closestEnemy = 0;
        float distance = Vector3.Distance(transform.position, destinations[0].transform.position);
        for(int i = 1; destinations.Count > i; i++){
            if(distance > Vector3.Distance(transform.position, destinations[i].transform.position)){
                distance = Vector3.Distance(transform.position, destinations[i].transform.position);
                closestEnemy = i;
            }
        }
        target = destinations[closestEnemy];
    }

    // Update is called once per frame
    void Update()
    {
        int closestEnemy = 0;
        float distance = Vector3.Distance(transform.position, destinations[0].transform.position);
        for(int i = 1; destinations.Count > i; i++){
            if(distance > Vector3.Distance(transform.position, destinations[i].transform.position)){
                distance = Vector3.Distance(transform.position, destinations[i].transform.position);
                closestEnemy = i;
            }
        }
        target = destinations[closestEnemy];
        Debug.Log("Target" + target);
    }

    public Transform GetClosest(){
        Debug.Log("Target" + target);
        return target.transform;
    }
}
