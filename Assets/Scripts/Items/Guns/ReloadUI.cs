using System.Collections;
using System.Runtime.InteropServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class ReloadUI : MonoBehaviour
{
    [SerializeField] InventoryHandler localPlayerInventory;
    [SerializeField] SpriteRenderer reloadBarRenderer;
    [SerializeField] SpriteRenderer fillRenderer;
    [SerializeField] GameObject reloadFill;
    [SerializeField] float fadeDuration;
    private IReloadableItem reloadableItem;

    void OnValidate()
    {
        localPlayerInventory = transform.parent.GetComponent<InventoryHandler>();
    }

    void Awake()
    {
        localPlayerInventory.OnItemSwapEvent += OnItemSwap;
        localPlayerInventory.OnItemPickedUpEvent += OnItemPickedUp;
    }

    void OnDestroy()
    {
        localPlayerInventory.OnItemSwapEvent -= OnItemSwap;
        localPlayerInventory.OnItemPickedUpEvent -= OnItemPickedUp;
        reloadableItem.OnReloadEvent -= OnItemReload;
        Debug.Log("Reload Unsubscribed Sucessfully");
    }

    void OnItemPickedUp(object sender, InventoryHandler.ItemPickedUpEventArgs e)
    {
        CheckItem(e.Item);
    }

    void OnItemSwap(object sender, InventoryHandler.ItemSwappedEventArgs e)
    {
        CheckItem(e.CurrentItem);
    }

    void CheckItem(IItem item)
    {
        if (this.reloadableItem != null)
            this.reloadableItem.OnReloadEvent -= OnItemReload;

        if (item is not IReloadableItem reloadableItem)
            return;

        this.reloadableItem = reloadableItem;
        this.reloadableItem.OnReloadEvent += OnItemReload;
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
