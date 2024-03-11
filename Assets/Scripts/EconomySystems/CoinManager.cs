using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] TMP_Text _text;

    void Awake()
    {
        InventoryManager.instance.OnInventoryUpdate += UpdateCoinCounter;
    }
    void OnDestroy()
    {
        InventoryManager.instance.OnInventoryUpdate -= UpdateCoinCounter;
    }

    void UpdateCoinCounter(Item item)
    {
            Debug.Log(item.itemType);
        if (item.itemType == InventoryItem.Coin)
        {
            Debug.Log(item.currentStored.ToString());
            _text.text = item.currentStored.ToString();
        }
    }
}
