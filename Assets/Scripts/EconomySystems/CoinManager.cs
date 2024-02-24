using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public int amount = 0;

    [SerializeField] TMP_Text _text;


    public void AddCoin(int a)
    {
        amount += a;
        _text.text = amount.ToString();
    }

    public bool SubCoin(int a)
    {
        //Return false if amount is exceed current amount (avoid negative)
        if (a > amount)
            return false;
        amount -= a;
        _text.text = amount.ToString();
        return true;
    }
}
