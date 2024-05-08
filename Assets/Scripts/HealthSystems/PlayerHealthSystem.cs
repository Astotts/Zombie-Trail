using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSystem : HealthSystem
{
    //Declaration
    public Slider healthBar;

    //Health Visuals
    [SerializeField] private Image sprite; //Holds Health and Health Background
    [SerializeField] private Color displayColor; //Holds Health and Health Background
    [SerializeField] private float singleFlashTime;
    [SerializeField] private float flashCycles;

    //Screen Visuals
    [SerializeField] private Image[] bloodEffect;
    [SerializeField] private Color bloodEffectColor; 
    [SerializeField] private float waitForFade;
    [SerializeField] private float timeToFade;

    void Awake() {
        currentHealth.OnValueChanged += AlterHealthClientRpc;
    }

    void OnDisable() {
        currentHealth.OnValueChanged -= AlterHealthClientRpc;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GameObject healthBarGO = GameObject.FindGameObjectWithTag("PlayerHealth");
        healthBar = healthBarGO.GetComponent<Slider>();
        sprite = healthBarGO.transform.GetChild(0).GetComponent<Image>();
        GameObject[] bloodEffectGO = GameObject.FindGameObjectsWithTag("BloodEffect");
        for (int i = 0; i < bloodEffect.Length; i++) {
            bloodEffect[i] = bloodEffectGO[i].GetComponent<Image>();
        }
        healthBar.value = maxHealth;
        if (!IsServer) return;
        // Assigning currentHealth.Value & healthBar to the value of maxHealth
        currentHealth.Value = maxHealth;
        
    }
    [Rpc(SendTo.Server)]
    public override void AlterHealthServerRpc(int amount)
    {
        currentHealth.Value += amount;
    }

    
    [Rpc(SendTo.ClientsAndHost)]
    private void AlterHealthClientRpc(int prev, int curr)
    {
        if (!IsOwner) return;
        StopCoroutine("ScreenEffect");
        //Debug.Log(-(((float)currentHealth.Value - (float)maxHealth) / (float)maxHealth));
        for(int i = 0; bloodEffect.Length > i; i++){
            bloodEffect[i].color = new Color(bloodEffectColor.r,bloodEffectColor.g,bloodEffectColor.b, -(((float)currentHealth.Value - (float)maxHealth) / (float)maxHealth)); 
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
        Color startingColor = bloodEffect[0].color;
        while(elapsed <= timeToFade){
            elapsed += Time.deltaTime;
            for(int i = 0; bloodEffect.Length > i; i++){
                bloodEffect[i].color = Color.Lerp(startingColor, Color.clear, elapsed / timeToFade);
            }
            yield return null;
        }
        yield break;
    }
}
