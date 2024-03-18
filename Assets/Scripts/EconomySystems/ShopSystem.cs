using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    [SerializeField] List<ShopSection> shopSections;
    [SerializeField] GameObject shopUI;
    bool active = false;
    bool isOpen = false;
    void Awake()
    {
        GameManager.OnStateChange += Spawn;
        GameManager.OnStateChange += Despawn;
    }

    void Spawn(GameState state)
    {
        if (state != GameState.WaveStart) return;
        gameObject.SetActive(true);
    }

    void Despawn(GameState state)
    {
        if (state != GameState.WaitStart) return;
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D target)
    {
        if (target.gameObject.CompareTag("Player"))
        {
            active = true;
        }
    }

    void OnTriggerExit2D(Collider2D target)
    {
        if (target.gameObject.CompareTag("Player"))
        {
            active = false;
        }
    }
    void OnEnable()
    {
        shopUI.SetActive(true);
    }
    void OnDisable()
    {
        shopUI.SetActive(false);
        shopUI.transform.localScale = new Vector3 (0, 0, 0);
    }

    void Update()
    {
        if (active && Input.GetKeyDown(KeyCode.Space))
        {
            if (isOpen)
                CloseShop();
            else
                OpenShop();
        }
    }

    void OpenShop()
    {
        shopUI.transform.localScale = new Vector3(8, 8, 1);
        isOpen = true;
    }

    void CloseShop()
    {
        shopUI.transform.localScale = new Vector3(0, 0, 0);
        isOpen = false;
    }
}