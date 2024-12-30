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

    public ulong PlayerID { get; set; }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        base.OnNetworkSpawn();
        EventManager.EventHandler.OnGunReloadEvent += OnItemReload;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        EventManager.EventHandler.OnGunReloadEvent -= OnItemReload;
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
        Debug.Log("reached");
        StartCoroutine(Reloading(reloadTime));
    }

    IEnumerator Reloading(float reloadTime)
    {
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
        StartCoroutine(HideReloadStatus());
    }

    IEnumerator HideReloadStatus()
    {
        float statusElapsed = 0f;
        while (statusElapsed <= fadeDuration)
        {
            statusElapsed += Time.deltaTime;
            float value = Mathf.Lerp(1f, 0f, statusElapsed / fadeDuration);
            SetOpacity(value);
            yield return null;
        }
        yield break;
    }


    public void SetOpacity(float a)
    {
        reloadBarRenderer.color = new Color(1, 1, 1, a);
        fillRenderer.color = new Color(1, 1, 1, a);
    }
}
