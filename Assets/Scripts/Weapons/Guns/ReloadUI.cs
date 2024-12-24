using System.Collections;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ReloadUI : MonoBehaviour
{
    public static ReloadUI Instance { get; private set; }
    [SerializeField] SpriteRenderer reloadBarRenderer;
    [SerializeField] SpriteRenderer fillRenderer;
    [SerializeField] GameObject reloadFill;
    [SerializeField] float fadeDuration;

    private IOnReloadEffectItem reloadableItem;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InventoryManager.Instance.OnItemSwapEvent += OnItemSwap;
    }

    void Destroy()
    {
        Instance = null;
        InventoryManager.Instance.OnItemSwapEvent -= OnItemSwap;
        reloadableItem.OnReloadEvent -= OnItemReload;
    }

    private void OnItemSwap(object sender, InventoryManager.ItemSwappedEventArgs e)
    {
        if (reloadableItem != null)
            reloadableItem.OnReloadEvent -= OnItemReload;
        reloadableItem = (IOnReloadEffectItem)e.CurrentItem;
        reloadableItem.OnReloadEvent += OnItemReload;
    }

    void OnItemReload(object sender, float reloadTime)
    {
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
