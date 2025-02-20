using System;
using System.Collections;
using System.Runtime.InteropServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Build.Player;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using static EventManager;

public class ReloadUI : NetworkBehaviour
{
    [SerializeField] SpriteRenderer reloadBarRenderer;
    [SerializeField] SpriteRenderer fillRenderer;
    [SerializeField] GameObject reloadFill;
    [SerializeField] float fadeDuration;

    Coroutine reloadCoroutine;
    Coroutine fadeCoroutine;

    bool isReloading;
    bool isFading;

    public ulong PlayerID { get; set; }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        EventManager.EventHandler.OnGunReloadEvent += OnItemReload;
        EventManager.EventHandler.OnGunReloadInterruptedEvent += OnReloadInterrupt;
        base.OnNetworkSpawn();
    }
    public override void OnNetworkDespawn()
    {
        EventManager.EventHandler.OnGunReloadEvent -= OnItemReload;
        EventManager.EventHandler.OnGunReloadInterruptedEvent -= OnReloadInterrupt;
        base.OnNetworkDespawn();
    }

    private void OnReloadInterrupt(object sender, GunReloadInterruptedEventArgs e)
    {
        if (e.PlayerID != this.PlayerID)
            return;
        PlayFadeEffectClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void PlayFadeEffectClientRpc()
    {
        fadeCoroutine = StartCoroutine(HideReloadStatus());
    }

    private void OnItemReload(object sender, GunReloadEventArgs e)
    {
        if (e.PlayerID != this.PlayerID)
            return;
        PlayReloadEffectClientRpc(e.ReloadTime);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void PlayReloadEffectClientRpc(float reloadTime)
    {
        reloadCoroutine = StartCoroutine(Reloading(reloadTime));
    }

    IEnumerator Reloading(float reloadTime)
    {
        if (isFading)
            StopCoroutine(fadeCoroutine);
        if (isReloading)
            StopCoroutine(reloadCoroutine);
        isFading = false;
        isReloading = true;
        reloadFill.transform.localScale = Vector2.zero;
        SetOpacity(1);
        float reloadElapsed = 0;
        float reloadValue;
        while (reloadElapsed < reloadTime)
        {
            reloadElapsed += Time.deltaTime;
            reloadValue = reloadElapsed / reloadTime;
            // Debug.Log(reloadValue);
            reloadFill.transform.localScale = new Vector3(reloadValue, 1, 0);
            yield return null;
        }
        yield return null;
        isReloading = false;
        fadeCoroutine = StartCoroutine(HideReloadStatus());
    }

    IEnumerator HideReloadStatus()
    {
        if (isFading)
            StopCoroutine(fadeCoroutine);
        if (isReloading)
            StopCoroutine(reloadCoroutine);
        isReloading = false;
        isFading = true;
        float statusElapsed = 0f;
        while (statusElapsed <= fadeDuration)
        {
            statusElapsed += Time.deltaTime;
            float value = Mathf.Lerp(1f, 0f, statusElapsed / fadeDuration);
            SetOpacity(value);
            yield return null;
        }
        yield return null;
        isFading = false;
    }


    public void SetOpacity(float a)
    {
        reloadBarRenderer.color = new Color(1, 1, 1, a);
        fillRenderer.color = new Color(1, 1, 1, a);
    }
}
