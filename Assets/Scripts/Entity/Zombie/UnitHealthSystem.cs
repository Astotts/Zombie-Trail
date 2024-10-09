using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class UnitHealthSystem : NetworkBehaviour, IDamageable
{
    //Health Functionality
    [SerializeField] private Transform bar;

    //Visual Elements
    [SerializeField] private SpriteRenderer[] sprites; //Holds Health and Health Background
    [SerializeField] private Color[] displayColor; //Holds Health and Health Background
    [SerializeField] private float singleFlashTime;
    [SerializeField] private float flashCycles;
    [SerializeField] private float fadeDuration;
    [SerializeField] NetworkObject networkObject;

    //Sound Functionality
    [SerializeField] private string[] sounds;

    //Variable
    [SerializeField] private int maxHealth = 10;
    private NetworkVariable<float> currentHealth = new();

    private IEnumerator healthFlashing;
    private IEnumerator hideHealth;
    private bool isDead => currentHealth.Value <= 0;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost)
            currentHealth.Value = maxHealth;

        currentHealth.OnValueChanged += PlayOnDamagedEffect;
    }

    void Start()
    {
        healthFlashing = HealthFlashing();
        hideHealth = HideHealth();
    }

    public void PlayOnDamagedEffect(float previous, float current)
    {
        StopCoroutine(healthFlashing);
        StopCoroutine(hideHealth);
        for (int i = 0; sprites.Length > i; i++)
        {
            sprites[i].color = displayColor[i];
        }

        if (currentHealth.Value > 0)
        {
            AudioManager.Instance.PlaySFX(sounds[UnityEngine.Random.Range(10, 13)], UnityEngine.Random.Range(0.7f, 1.1f));
            SetSize(((float)currentHealth.Value / (float)maxHealth)); //Since health variables are ints must cast to float values
            StartCoroutine(HealthFlashing());
        }
        else
        {
            AudioManager.Instance.PlaySFX(sounds[UnityEngine.Random.Range(0, 9)], UnityEngine.Random.Range(0.7f, 1.1f));
            SetSize(0f); //Size is normalized so 0 is 0% health
        }
    }

    public void Die()
    {
        StopCoroutine(healthFlashing);
        StopCoroutine(hideHealth);
        networkObject.Despawn();
    }

    private void SetSize(float sizeNormalized)
    {
        bar.localScale = new Vector3(sizeNormalized, 1f);
    }


    IEnumerator HealthFlashing()
    {
        float elapsed = 0f;
        for (int i = 0; i <= flashCycles; i++)
        {
            StopCoroutine(hideHealth);
            while (elapsed <= singleFlashTime)
            { //Turn to White
                elapsed += Time.deltaTime;
                Color color = Color.Lerp(displayColor[0], Color.white, (elapsed / (singleFlashTime / 2f)));
                sprites[0].color = new Color(color.r, color.g, color.b, color.a);
                yield return null;
            }
            elapsed = 0f;
            while (elapsed <= singleFlashTime)
            { //Turn to Health Color
                elapsed += Time.deltaTime;
                Color color = Color.Lerp(Color.white, displayColor[0], (elapsed / (singleFlashTime / 2f)));
                sprites[0].color = color;
                yield return null;
            }
            elapsed = 0f;
        }
        StartCoroutine(HideHealth());
        yield break;
    }

    IEnumerator HideHealth()
    {
        float elapsed = 0f;
        while (elapsed <= fadeDuration)
        {
            elapsed += Time.deltaTime;
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].color = new Color(displayColor[i].r, displayColor[i].g, displayColor[i].b, Mathf.Lerp(1f, 0f, (elapsed / fadeDuration)));
            }
            yield return null;
        }
        yield break;
    }

    // Only host is allowed to run this
    public void Damage(float amount)
    {
        currentHealth.Value -= amount;
        if (isDead)
            Die();
    }
}
