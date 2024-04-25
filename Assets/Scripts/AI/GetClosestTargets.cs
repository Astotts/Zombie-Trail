using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GetClosestTargets : MonoBehaviour
{
    
    private List<GameObject> destinations;
    private List<Collider2D> colliders;
    private List<float> distances;
    [SerializeField] string[] tagToFind;
    private GameObject target;
    private float targetDistance;
    // Start is called before the first frame update
    void Awake()
    {

        destinations = new List<GameObject>();
        for(int i = 0; tagToFind.Length > i; i++){
            destinations.AddRange(GameObject.FindGameObjectsWithTag(tagToFind[i]));
        }
        
        colliders = new List<Collider2D>();
        for(int i = 0; destinations.Count > i; i++){
            colliders.Add(destinations[i].GetComponent<Collider2D>());
        }

        distances = new List<float>();
        for(int i = 0; destinations.Count > i; i++){
            distances.Add((colliders[i].ClosestPoint((Vector2)transform.position) - (Vector2)transform.position).magnitude);
        }

        int closestEnemy = 0;
        float distance = distances[0];

        for(int i = 0; destinations.Count - 1 > i; i++){
            if(distance > distances[i]){
                distance = distances[i];
                closestEnemy = i;
            }
        }
        target = destinations[closestEnemy];
        targetDistance = distances[closestEnemy];
    }

    // Update is called once per frame
    void Update()
    {
        FindClosest();
    }

    private void FindClosest(){
        colliders.RemoveAll(item => item == null);
        destinations.RemoveAll(item => item == null);
        int closestEnemy = 0;
        
        for(int i = 0; destinations.Count > i; i++){
            distances[i] = (colliders[i].ClosestPoint((Vector2)transform.position) - (Vector2)transform.position).magnitude;
            //Debug.DrawRay(transform.position, colliders[i].ClosestPoint((Vector2)transform.position) - (Vector2)transform.position, Color.green, 0.1f);
            //Debug.DrawRay(colliders[i].ClosestPoint((Vector2)transform.position), colliders[i].transform.up, Color.red, 0.1f);
            
        }

        float distance = distances[0];

        for(int i = 0; destinations.Count > i; i++){
            if(distance > distances[i]){
                distance = distances[i];
                closestEnemy = i;
            }
        }
        target = destinations[closestEnemy];
        targetDistance = distances[closestEnemy];
    }

    public Transform GetClosest(){
        return target.transform;
    }

    public float GetDistance(){
        return targetDistance;
    }
}
