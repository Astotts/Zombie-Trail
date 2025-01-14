using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.InputSystem.Controls;
using UnityEngine.U2D;

public class HealthEffect : NetworkBehaviour
{
    [SerializeField] HealthVFXStats VFXStats;
    [SerializeField] HealthSFXStats SFXStats;
    [SerializeField] SpriteRenderer bodySprite;
    [SerializeField] List<SpriteRenderer> healthBarSprites;
    [SerializeField] SpriteRenderer healthBarFill;


    Color originalFillColor;
    Coroutine redFlashCoroutine;
    Coroutine hideHealthCoroutine;
    Coroutine healthFlashCoroutine;
    Coroutine healthDecreaseCoroutine;
    bool isRedFlashing;
    bool isHidingHealth;
    bool isHealthFlashing;
    bool isHealthDecreasing;

    void Awake()
    {
        originalFillColor = healthBarFill.color;
        HideHealthImmediate();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
            ResetZombie();
    }

    public void PlayOnDamaged(string health)
    {
        int indexOfColon = health.IndexOf(':');
        int indexOfComma = health.IndexOf(',');

        float previous = float.Parse(health[..indexOfColon]);
        float current = float.Parse(health[(indexOfColon + 1)..indexOfComma]);
        float max = float.Parse(health[(indexOfComma + 1)..]);

        PlayOnDamagedEffect(previous, current, max);
    }

    // Client and host(also server) behaviours
    public void PlayOnDamagedEffect(float previous, float current, float max)
    {
        // StopCoroutine(healthFlashing);
        // StopCoroutine(hideHealth);
        // for (int i = 0; sprites.Length > i; i++)
        // {
        //     sprites[i].color = displayColor[i];
        // }

        if (current > 0)
        {
            RandomPitchSound sound = SFXStats.OnDamagedSFX[UnityEngine.Random.Range(0, SFXStats.OnDamagedSFX.Length)];
            AudioManager.Instance.PlaySFX(sound.SoundID, UnityEngine.Random.Range(sound.MinPitch, sound.MaxPitch));

            ShowHealthImmediate();

            healthDecreaseCoroutine = StartCoroutine(HealthDecreasing(previous / max, current / max));
            healthFlashCoroutine = StartCoroutine(HealthFlashing());
            redFlashCoroutine = StartCoroutine(RedFlashing());
        }
        else
        {
            SetSize(0f); //Size is normalized so 0 is 0% health
            if (isHealthDecreasing)
                StopCoroutine(healthDecreaseCoroutine);
            if (isHealthFlashing)
                StopCoroutine(healthFlashCoroutine);
            if (isRedFlashing)
                StopCoroutine(redFlashCoroutine);
            if (isHidingHealth)
                StopCoroutine(hideHealthCoroutine);
        }
    }

    void ResetZombie()
    {
        bodySprite.color = Color.white;
        SetSize(1);
        healthBarFill.color = originalFillColor;
        HideHealthImmediate();
    }

    private void SetSize(float sizeNormalized)
    {
        healthBarFill.gameObject.transform.localScale = new Vector3(sizeNormalized, 1f);
    }

    IEnumerator RedFlashing()
    {
        if (isRedFlashing)
            StopCoroutine(redFlashCoroutine);
        isRedFlashing = true;
        float flashTime = VFXStats.HealthBarSingleFlashTime / 2;
        float elapsed = 0f;
        while (elapsed <= flashTime)
        {
            elapsed += Time.deltaTime;
            float step = elapsed / flashTime;
            Color color = Color.Lerp(Color.white, Color.red, step);
            bodySprite.color = color;
            yield return null;
        }
        elapsed = flashTime;
        while (elapsed > 0)
        {
            elapsed -= Time.deltaTime;
            float step = elapsed / flashTime;
            Color color = Color.Lerp(Color.white, Color.red, step);
            bodySprite.color = color;
            yield return null;
        }

        isRedFlashing = false;
    }

    IEnumerator HealthDecreasing(float previous, float current)
    {
        if (isHealthDecreasing)
            StopCoroutine(healthDecreaseCoroutine);
        isHealthDecreasing = true;
        float decreaseTime = VFXStats.HealthBarDecreasingTime;
        float elapsed = 0;
        while (elapsed < decreaseTime)
        {
            elapsed += Time.deltaTime;
            float step = elapsed / decreaseTime;
            SetSize(Mathf.Lerp(previous, current, step));
            yield return null;
        }
        // Normalized size
        SetSize(current);
        isHealthDecreasing = false;
    }

    IEnumerator HealthFlashing()
    {
        if (isHealthFlashing)
            StopCoroutine(healthFlashCoroutine);
        if (isHidingHealth)
            StopCoroutine(hideHealthCoroutine);

        isHealthFlashing = true;

        for (int i = 0; i < VFXStats.HealthBarFlashCycles; i++)
        {
            float flashTime = VFXStats.HealthBarSingleFlashTime / 2;
            float elapsed = 0f;
            while (elapsed <= flashTime)
            { //Turn to White
                elapsed += Time.deltaTime;
                float step = elapsed / flashTime;
                Color color = Color.Lerp(originalFillColor, Color.white, step);
                healthBarFill.color = color;
                yield return null;
            }
            while (elapsed > 0)
            { //Turn to Health Color
                elapsed -= Time.deltaTime;
                float step = elapsed / flashTime;
                Color color = Color.Lerp(originalFillColor, Color.white, step);
                healthBarFill.color = color;
                yield return null;
            }
        }
        isHealthFlashing = false;
        hideHealthCoroutine = StartCoroutine(HideHealth());
    }

    void ShowHealthImmediate()
    {
        foreach (SpriteRenderer spriteRenderer in healthBarSprites)
        {
            SetOpacity(spriteRenderer, 1);
        }
    }
    void HideHealthImmediate()
    {
        foreach (SpriteRenderer spriteRenderer in healthBarSprites)
        {
            SetOpacity(spriteRenderer, 0);
        }
    }

    IEnumerator HideHealth()
    {
        if (isHidingHealth)
            StopCoroutine(hideHealthCoroutine);
        if (isHealthFlashing)
            StopCoroutine(healthFlashCoroutine);
        isHidingHealth = true;

        yield return new WaitForSeconds(VFXStats.HealthBarTimeBeforeFade);

        float fadeTime = VFXStats.HealthBarFadeTime;
        float elapsed = 0f;
        while (elapsed <= fadeTime)
        {
            elapsed += Time.deltaTime;
            foreach (SpriteRenderer spriteRenderer in healthBarSprites)
            {
                SetOpacity(spriteRenderer, Mathf.Lerp(1f, 0f, elapsed / fadeTime));
            }
            yield return null;
        }

        isHidingHealth = false;
    }

    void SetOpacity(SpriteRenderer spriteRenderer, float opacity)
    {
        Color originalColor = spriteRenderer.color;
        Color newColor = new(originalColor.r, originalColor.g, originalColor.b, opacity);

        spriteRenderer.color = newColor;
    }
}