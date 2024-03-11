using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopSection : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text priceText;
    [SerializeField] GameObject priceObject;
    [SerializeField] TMP_Text amountText;
    InventoryItem itemId;
    int amount;
    int price;
    public int GetPrice()
    {
        return price;
    }
    void OnEnable()
    {
        itemId = (InventoryItem)Random.Range(0,6);
        amount = Random.Range(1,5);
        price = Random.Range(1,100);
        Item item = InventoryManager.allItemsInGame.Find(x => x.itemType == itemId);
        itemImage.sprite = item.icon;
        nameText.text = item.itemName;
        priceText.text = GetString(price);
        button.onClick.AddListener(BuyItem);
        AlignPriceCenter();
        amountText.text = "<size=4>x<size=5>" + amount.ToString();
    }
    void OnDisable()
    {
        button.onClick.RemoveListener(BuyItem);
    }
    string GetString(int price)
    {
        string output;

        if (price / 1e12 >= 1)
            output = (price / 1e12).ToString("F0") + "T";
        else if ((int)price / 1e9 >= 1)
            output = (price / 1e9).ToString("F0") + "B";
        else if ((int)price / 1e6 >= 1)   
            output = (price / 1e6).ToString("F0") + "M";
        else if ((int)price / 1e3 >= 1)
            output = (price / 1e3).ToString("F0") + "K";
        else
            output = price.ToString();

        return output;
    }

    void AlignPriceCenter()
    {
        float length = (5 + priceText.preferredWidth) / 2 * -1;
        priceObject.transform.localPosition = new Vector3(length, priceObject.transform.localPosition.y, 0);
    }
    public ShopSection(InventoryItem id, int a, int p)
    {
        itemId = id;
        amount = a;
        price = p;
    }
    public ItemCount GetItemCount()
    {
        return new ItemCount(itemId, amount);
    }
    public ItemCount GetPriceItemCount()
    {
        return new ItemCount(InventoryItem.Coin,price);
    }

    public void BuyItem()
    {
        if (InventoryManager.instance.SubtractFromInventory(GetPriceItemCount()))
            InventoryManager.instance.AddToInventory(GetItemCount());
        else
            Debug.Log("Not Enough Coin");
    }
}
