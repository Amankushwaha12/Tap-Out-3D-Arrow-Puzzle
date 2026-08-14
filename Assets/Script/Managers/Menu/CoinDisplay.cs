using TMPro;
using UnityEngine;

public class CoinDisplay : MonoBehaviour
{
    public TextMeshProUGUI coinText, coinText_1;

    public void CoinTextUpdate()
    {
        coinText.text = Wallet.Coins.ToString();
        coinText_1.text = Wallet.Coins.ToString();
        Debug.Log("Coin Text Update Called." + Wallet.Coins);
    }
}
