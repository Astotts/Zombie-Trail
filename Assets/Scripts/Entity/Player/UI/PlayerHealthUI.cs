using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : NetworkBehaviour
{
    [SerializeField] Slider healthSlider;
    [SerializeField] float decreaseTime;

    Coroutine decreaseCoroutine;
    bool isDecreasing;

    void Awake()
    {
        EventManager.EventHandler.OnPlayerDamagedEvent += OnPlayerDamaged;
        EventManager.EventHandler.OnPlayerHealthLoadedEvent += OnPlayerHealthLoaded;
    }

    private void OnPlayerHealthLoaded(object sender, EventManager.PlayerHealthLoadedEventArgs e)
    {
        SetHealthClientRpc(e.CurrentHealth, e.MaxHealth, RpcTarget.Single(e.PlayerID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void SetHealthClientRpc(float current, float max, RpcParams rpcParams)
    {
        SetHealthValue(current, max);
    }

    void SetHealthValue(float current, float max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = current / max;
    }

    private void OnPlayerDamaged(object sender, EventManager.PlayerDamagedEventArgs e)
    {
        PlayDecreaseAnimationClientRpc(e.CurrentHealth, e.MaxHealth, RpcTarget.Single(e.PlayerID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void PlayDecreaseAnimationClientRpc(float current, float max, RpcParams rpcParams)
    {
        if (isDecreasing)
            StopCoroutine(decreaseCoroutine);
        decreaseCoroutine = StartCoroutine(HealthDecreaseAnimation(current / max * healthSlider.maxValue));
    }

    IEnumerator HealthDecreaseAnimation(float newValue)
    {
        isDecreasing = true;

        float elapsed = 0;
        float current = healthSlider.value;
        while (elapsed < decreaseTime)
        {
            elapsed += Time.deltaTime;
            healthSlider.value = Mathf.Lerp(current, newValue, elapsed / decreaseTime);
            yield return null;
        }

        isDecreasing = false;
    }
}
