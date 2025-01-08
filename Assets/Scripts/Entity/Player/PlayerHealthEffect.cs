using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthEffect : MonoBehaviour
{
    [SerializeField] PlayerVFXStats VFXStats;
    [SerializeField] PlayerSFXStats SFXStats;
    [SerializeField] SpriteRenderer bodySprite;

    Coroutine redFlashCoroutine;
    bool isRedFlashing;

    // public void PlayOnDamaged(string health)
    // {
    //     int indexOfColon = health.IndexOf(':');
    //     int indexOfComma = health.IndexOf(',');

    //     float previous = float.Parse(health[..indexOfColon]);
    //     float current = float.Parse(health[(indexOfColon + 1)..indexOfComma]);
    //     float max = float.Parse(health[(indexOfComma + 1)..]);

    //     PlayOnDamagedEffect(current, max);
    // }

    // Client and host(also server) behaviours
    public void PlayOnDamagedEffect(float current, float max)
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


            redFlashCoroutine = StartCoroutine(RedFlashing(current, max));
        }
        else
        {
            if (isRedFlashing)
                StopCoroutine(redFlashCoroutine);
        }
    }

    IEnumerator RedFlashing(float current, float max)
    {
        if (isRedFlashing)
            StopCoroutine(redFlashCoroutine);
        isRedFlashing = true;
        float flashTime = VFXStats.OnDamagedRedFlashTime / 2;
        float elapsed = 0f;
        Color currentColor = bodySprite.color;
        Color newColor = Color.Lerp(Color.white, Color.red, current / max);
        while (elapsed <= flashTime)
        {
            elapsed += Time.deltaTime;
            float step = elapsed / flashTime;
            Color color = Color.Lerp(currentColor, Color.red, step);
            bodySprite.color = color;
            yield return null;
        }
        elapsed = flashTime;
        while (elapsed > 0)
        {
            elapsed -= Time.deltaTime;
            float step = elapsed / flashTime;
            Color color = Color.Lerp(Color.white, newColor, step);
            bodySprite.color = color;
            yield return null;
        }

        isRedFlashing = false;
    }
}
