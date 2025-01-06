using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieHealthSystem : NetworkBehaviour
{
    //Health Functionality
    // [SerializeField] private Transform bar;
    [SerializeField] private ZombiePrefabSO zombiePrefabSO;

    //Visual Elements
    // [SerializeField] private SpriteRenderer[] sprites; //Holds Health and Health Background
    // [SerializeField] private Color[] displayColor; //Holds Health and Health Background
    [SerializeField] NetworkObject networkObject;
    [SerializeField] HealthBarStatsSO healthBarStats;
    [SerializeField] SpriteRenderer zombieSprite;

    //Sound Functionality
    [SerializeField] private string[] sounds;

    private float currentHealth;

    // private IEnumerator healthFlashing;
    // private IEnumerator hideHealth;
    private bool IsDead => currentHealth <= 0;
    private IZombie zombie;

    Coroutine redFlashCoroutine;
    bool isRedFlashing;

    // Server Behaviours

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            currentHealth = zombie.Stats.MaxHealth;

        base.OnNetworkSpawn();
    }

    public void Init(IZombie zombie)
    {
        this.zombie = zombie;
    }

    public void Damage(float amount)
    {
        currentHealth -= amount;
        PlayOnDamagedEffectClientRpc(currentHealth);

        if (IsDead)
            Die();
    }

    public void Die()
    {
        // StopCoroutine(healthFlashing);
        // StopCoroutine(hideHealth);
        if (isRedFlashing)
            StopCoroutine(redFlashCoroutine);

        GameObject prefab = zombiePrefabSO.GetZombiePrefabFromType(zombie.Type);
        NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject, prefab);

        networkObject.Despawn(false);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayOnDamagedEffectClientRpc(float current)
    {
        PlayOnDamagedEffect(current);
    }

    // Client and host(also server) behaviours

    public void PlayOnDamagedEffect(float current)
    {
        // StopCoroutine(healthFlashing);
        // StopCoroutine(hideHealth);
        // for (int i = 0; sprites.Length > i; i++)
        // {
        //     sprites[i].color = displayColor[i];
        // }

        if (currentHealth > 0)
        {
            AudioManager.Instance.PlaySFX(sounds[UnityEngine.Random.Range(10, 13)], UnityEngine.Random.Range(0.7f, 1.1f));
            // SetSize(((float)current / (float)zombie.Stats.MaxHealth)); //Since health variables are ints must cast to float values
            // StartCoroutine(HealthFlashing());
            redFlashCoroutine = StartCoroutine(RedFlashing());
        }
        else
        {
            AudioManager.Instance.PlaySFX(sounds[UnityEngine.Random.Range(0, 9)], UnityEngine.Random.Range(0.7f, 1.1f));
            // SetSize(0f); //Size is normalized so 0 is 0% health
        }
    }

    // private void SetSize(float sizeNormalized)
    // {
    //     bar.localScale = new Vector3(sizeNormalized, 1f);
    // }

    IEnumerator RedFlashing()
    {
        if (isRedFlashing)
            StopCoroutine(redFlashCoroutine);

        isRedFlashing = true;

        for (int i = 0; i <= healthBarStats.FlashCycles; i++)
        {
            float elapsed = 0f;
            while (elapsed <= healthBarStats.SingleFlashTime)
            {
                elapsed += Time.deltaTime;
                float step = elapsed / (healthBarStats.SingleFlashTime / 2);
                Color color = Color.Lerp(Color.white, Color.red, step);
                zombieSprite.color = color;
                yield return null;
            }
            while (elapsed > 0)
            {
                elapsed -= Time.deltaTime;
                float step = elapsed / (healthBarStats.SingleFlashTime / 2);
                Color color = Color.Lerp(Color.white, Color.red, step);
                zombieSprite.color = color;
                yield return null;
            }
        }

        isRedFlashing = false;
    }

    // IEnumerator HealthFlashing()
    // {
    //     float elapsed = 0f;
    //     for (int i = 0; i <= healthBarStats.FlashCycles; i++)
    //     {
    //         StopCoroutine(hideHealth);
    //         while (elapsed <= healthBarStats.SingleFlashTime)
    //         { //Turn to White
    //             elapsed += Time.deltaTime;
    //             Color color = Color.Lerp(displayColor[0], Color.white, (elapsed / (healthBarStats.SingleFlashTime / 2f)));
    //             sprites[0].color = new Color(color.r, color.g, color.b, color.a);
    //             yield return null;
    //         }
    //         elapsed = 0f;
    //         while (elapsed <= healthBarStats.SingleFlashTime)
    //         { //Turn to Health Color
    //             elapsed += Time.deltaTime;
    //             Color color = Color.Lerp(Color.white, displayColor[0], (elapsed / (healthBarStats.SingleFlashTime / 2f)));
    //             sprites[0].color = color;
    //             yield return null;
    //         }
    //         elapsed = 0f;
    //     }
    //     StartCoroutine(HideHealth());
    //     yield break;
    // }

    // IEnumerator HideHealth()
    // {
    //     float elapsed = 0f;
    //     while (elapsed <= healthBarStats.FadeDuration)
    //     {
    //         elapsed += Time.deltaTime;
    //         for (int i = 0; i < sprites.Length; i++)
    //         {
    //             sprites[i].color = new Color(displayColor[i].r, displayColor[i].g, displayColor[i].b, Mathf.Lerp(1f, 0f, (elapsed / healthBarStats.FadeDuration)));
    //         }
    //         yield return null;
    //     }
    //     yield break;
    // }
}