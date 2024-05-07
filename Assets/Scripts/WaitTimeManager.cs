using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WaitTimeManager : NetworkBehaviour
{
    [SerializeField] int waitTime = 60;
    [SerializeField] TMP_Text counter;
    [SerializeField] GameObject merchantGO;
    [SerializeField] RawImage blackScreen;
    [SerializeField] int fadeTime = 1;
    [SerializeField] TMP_Text waitingTextObj;
    [SerializeField] string waitingText = "Until Next Wave\nSkip? Y";
    
    bool flipped = false;
    bool active = false;
    bool fadeInEnded = false;
    bool fadeOutEnded = false;
    void Start()
    {
        GameManager.OnStateChange += ShoppingStart;
    }
    
    void OnDisable()
    {
        GameManager.OnStateChange -= ShoppingStart;
    }
    
    void ShoppingStart(GameState state)
    {
        if (state != GameState.WaveEnd)
            return;
        WaitProcessClientRpc();
    }
    [Rpc(SendTo.ClientsAndHost)]
    void WaitProcessClientRpc() {
        active = true;
        StartCoroutine(WaitProcess());
    }

    IEnumerator WaitProcess()
    {
        StartCoroutine(FadeOut());
        yield return new WaitUntil(() => fadeOutEnded);
        fadeOutEnded = false;
        merchantGO.SetActive(true);
        GameManager.StateUpdate(GameState.WaitStart);
        StartCoroutine(FadeIn());
        yield return new WaitUntil(() => fadeInEnded);
        fadeInEnded = false;
        StartCoroutine(Timer());
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        float start = 0f;
        float end = 1f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(start, end, elapsedTime / fadeTime);
            blackScreen.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }
        fadeOutEnded = true;
    }
    IEnumerator FadeIn()
    {
        float elapsedTime = 0;
        float start = 1f;
        float end = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(start, end, elapsedTime / fadeTime);
            blackScreen.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }
        fadeInEnded = true;
    }

    IEnumerator Timer()
    {
        counter.gameObject.SetActive(true);
        for (int i = waitTime; i > 0; i--)
        {
            counter.text = i.ToString();
            yield return new WaitForSeconds(1.0f);
            if (!active)
                break;
            FlipLetter();
        }
        StartCoroutine(FadeOut());
        yield return new WaitUntil(() => fadeOutEnded);
        fadeOutEnded = false;
        GameManager.StateUpdate(GameState.WaitEnd);
        counter.gameObject.SetActive(false);
        StartCoroutine(FadeIn());
        fadeOutEnded = false;
        merchantGO.SetActive(false);
    }

    void FlipLetter()
    {
        if (flipped)
        {
            waitingText = "Until Next Wave\nSkip? <u>Y</u>";
            waitingTextObj.text = waitingText;
            flipped = false;
        }
        else 
        {
            waitingText = "Until Next Wave\nSkip? Y";
            waitingTextObj.text = waitingText;
            flipped = true;
        }
    }

    void Update()
    {
        if (active && Input.GetKeyDown(KeyCode.Y))
            SetActiveClientRpc(false);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetActiveClientRpc(bool active) {
        this.active = active;
    }
}
