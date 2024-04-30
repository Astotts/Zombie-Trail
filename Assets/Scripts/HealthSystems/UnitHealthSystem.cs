using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class UnitHealthSystem : HealthSystem
{
    //Health Functionality
    [SerializeField] private Transform bar;

    //Visual Elements
    [SerializeField] private SpriteRenderer[] sprites; //Holds Health and Health Background
    [SerializeField] private Color[] displayColor; //Holds Health and Health Background
    [SerializeField] private float singleFlashTime;
    [SerializeField] private float flashCycles;
    [SerializeField] private float fadeDuration;
    
    void Awake() {
        currentHealth.OnValueChanged += AlterHealthClientRpc;
    }
    void OnDisable() {
        currentHealth.OnValueChanged -= AlterHealthClientRpc;
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
            currentHealth.Value = maxHealth;
        SetSize(1f); //Size is normalized so 1 is 100% health
    }

    [Rpc(SendTo.Server)]
    public override void AlterHealthServerRpc(int amount) {
        currentHealth.Value += amount;
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void AlterHealthClientRpc(int prev, int curr) {
        StopCoroutine("HealthFlashing");
        StopCoroutine("HideHealth");
        for(int i = 0; sprites.Length > i; i++){
            sprites[i].color = displayColor[i];
        }
        if(curr > 0){
            SetSize(((float)curr / (float)maxHealth)); //Since health variables are ints must cast to float values
            //for(int i = sprites.Length; i > 0; i--){
                //sprite.color = new Color(displayColor.r, displayColor.g, displayColor.b, displayColor.a);
            //}
            StartCoroutine("HealthFlashing");
        }
        else {
            SetSize(0f); //Size is normalized so 0 is 0% health
            Die();
        }    
    }

    public override void Die(){
        if (!IsServer) return;
        StopCoroutine("HealthFlashing");
        StopCoroutine("HideHealth");
        GameManager.Instance.RemoveZombieFromList(gameObject);
        Destroy(gameObject);
    }

    private void SetSize(float sizeNormalized) {
        bar.localScale = new Vector3(sizeNormalized, 1f);
    }

    
    IEnumerator HealthFlashing(){
        float elapsed = 0f;
        for(int i = 0; i <= flashCycles; i++){
            StopCoroutine("HideHealth");
            while(elapsed <= singleFlashTime){ //Turn to White
                elapsed += Time.deltaTime;
                Color color = Color.Lerp(displayColor[0], Color.white, (elapsed / (singleFlashTime / 2f)));
                sprites[0].color = new Color(color.r, color.g, color.b, color.a);
                yield return null;
            }
            elapsed = 0f;
            while(elapsed <= singleFlashTime){ //Turn to Health Color
                elapsed += Time.deltaTime;
                Color color = Color.Lerp(Color.white, displayColor[0], (elapsed / (singleFlashTime / 2f)));
                sprites[0].color = color;
                yield return null;
            }
            elapsed = 0f;
        }
        StartCoroutine("HideHealth");
        yield break;
    }

    IEnumerator HideHealth(){
        float elapsed = 0f;
        while(elapsed <= fadeDuration){
            elapsed += Time.deltaTime;
            for(int i = 0; i < sprites.Length; i++){
                sprites[i].color = new Color(displayColor[i].r, displayColor[i].g, displayColor[i].b, Mathf.Lerp(1f, 0f, (elapsed / fadeDuration)));
            }
            yield return null;
        }
        yield break;
    }
}
