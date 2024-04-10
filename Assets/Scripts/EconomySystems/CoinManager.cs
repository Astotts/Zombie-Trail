using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] TMP_Text _text;

    void Start()
    {
        InventoryManager.instance.OnInventoryUpdate += UpdateCoinCounter;
    }
    void OnDestroy()
    {
        InventoryManager.instance.OnInventoryUpdate -= UpdateCoinCounter;
    }

    void UpdateCoinCounter(Item item)
    {
        if (item.itemType == InventoryItem.Coin)
        {
            _text.text = item.currentStored.ToString();
        }
    }
}
