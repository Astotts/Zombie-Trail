using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Droptable : MonoBehaviour
{
    [SerializeField] GameObject[] itemPrefab;
    [SerializeField] float[] chance;
    [SerializeField] float dropRange;
    
    void OnDestroy()
    {
        if (appClose) return;
        for (int i = 0; i < itemPrefab.Length; i++)
        {
            bool success;

            if (i >= chance.Length)
                success = true;
            else
                success = Chance(chance[i]);

            if (success)
            {
                Vector3 offset = GetRandomDropLocation();
                Instantiate(itemPrefab[i],transform.position + offset, transform.rotation);
            }
            
        }
    }
    Vector3 GetRandomDropLocation()
    {
        float x = Random.Range(-dropRange,dropRange);
        float y = Random.Range(-dropRange,dropRange);
        return new Vector3(x, y);
    }
    bool Chance(float chance)
    {
        float random = Random.Range(0, 1);
        return random < chance;
    }

    bool appClose = false;
    void OnApplicationQuit()
    {
        appClose = true;
    }
}
