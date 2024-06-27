using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealthSystem : HealthSystem
{
    //Health Visuals
    [SerializeField] private Color displayColor; //Holds Health and Health Background
    [SerializeField] private float singleFlashTime;
    [SerializeField] private float flashCycles;

    //Screen Visuals
    [SerializeField] private Color bloodEffectColor; 
    [SerializeField] private float waitForFade;
    [SerializeField] private float timeToFade;

    Image[] bloodEffects;
    Slider healthBar;
    Image sprite;

    public override void Start() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        NetworkManager.OnClientConnectedCallback += UpdateUIReference;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        NetworkManager.OnClientConnectedCallback -= UpdateUIReference;
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateUIReference(0);
        healthBar.value = maxHealth;
        if (!IsServer) return;
        // Assigning currentHealth.Value & healthBar to the value of maxHealth
        currentHealth.Value = maxHealth;
    }

    void UpdateUIReference(ulong id) {
        bloodEffects = PlayerUIManager.Instance.bloodEffects;
        healthBar = PlayerUIManager.Instance.healthBar;
        sprite = PlayerUIManager.Instance.healthBarSprite;
    }

    [Rpc(SendTo.Server)]
    public override void AlterHealthServerRpc(int amount)
    {
        currentHealth.Value += amount;
        AlterHealthClientRpc(currentHealth.Value);
    }

    
    [Rpc(SendTo.Owner)]
    private void AlterHealthClientRpc(int curr)
    {
        StopCoroutine("ScreenEffect");
        
        for(int i = 0; bloodEffects.Length > i; i++){
            bloodEffects[i].color = new Color(bloodEffectColor.r,bloodEffectColor.g,bloodEffectColor.b, -(((float)currentHealth.Value - (float)maxHealth) / (float)maxHealth)); 
        }

        StartCoroutine("HealthFlashing");
        StartCoroutine("ScreenEffect");
        healthBar.value = (float)curr / (float)maxHealth * 100f;

        // Check for death
        if (curr <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        StopCoroutine("HealthFlashing");
        // Death animation, game over screen, etc.
        Debug.LogWarning("You Are Dead.");

        //Removes gameObject
        //Destroy(gameObject);

        //!DEBUG RESET TO HEALTH DELETE LATER
        currentHealth.Value = maxHealth;
    }

    IEnumerator HealthFlashing(){
        float elapsed = 0f;
        for(int i = 0; i <= flashCycles; i++){
            while(elapsed <= singleFlashTime){ //Turn to White
                elapsed += Time.deltaTime;
                Color color = Color.Lerp(displayColor, Color.white, (elapsed / (singleFlashTime / 2f)));
                sprite.color = color;
                yield return null;
            }
            elapsed = 0f;
            while(elapsed <= singleFlashTime){ //Turn to Health Color
                elapsed += Time.deltaTime;
                Color color = Color.Lerp(Color.white, displayColor, (elapsed / (singleFlashTime / 2f)));
                sprite.color = color;
                yield return null;
            }
            elapsed = 0f;
        }
        
        yield break;
    }

    IEnumerator ScreenEffect(){
        float elapsed = 0f;
        while(elapsed <= waitForFade){
            elapsed += Time.deltaTime;
            yield return null;
        }
        elapsed = 0f;
        Color startingColor = bloodEffects[0].color;
        while(elapsed <= timeToFade){
            elapsed += Time.deltaTime;
            for(int i = 0; bloodEffects.Length > i; i++){
                bloodEffects[i].color = Color.Lerp(startingColor, Color.clear, elapsed / timeToFade);
            }
            yield return null;
        }
        yield break;
    }
}
