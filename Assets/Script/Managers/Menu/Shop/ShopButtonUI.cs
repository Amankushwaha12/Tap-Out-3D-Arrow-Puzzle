using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopButtonUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage, backgroundImage;
    public TextMeshProUGUI priceText;
    public CustomButton buyButton;

    private ShopItem currentItem;
    private ShopElementLoader shopLoader;

    public void Setup(ShopItem item, ShopElementLoader loader)
    {
        currentItem = item;
        shopLoader = loader;

        iconImage.sprite = item.icon;
        UpdateVisuals();

        buyButton.onClick.AddListener(OnBuyClicked);
    }

    public void UpdateVisuals()
    {
        // Fetch status from the updated ShopData logic which now reads from JSON
        ItemStatus status = ShopData.GetItemStatus(currentItem);

        switch (status)
        {
            case ItemStatus.Unlocked_Equipped:
                priceText.text = "EQUIPPED";
                buyButton.interactable = false; // Cannot click an equipped item
                backgroundImage.color = Color.grey;
                break;

            case ItemStatus.Unlocked_Owned:
                priceText.text = "SELECT";
                buyButton.interactable = true; // Click to equip
                backgroundImage.color = Color.white;
                break;

            case ItemStatus.Locked_Buyable:
                priceText.text = currentItem.price.ToString();
                buyButton.interactable = true;
                backgroundImage.color = Color.white;
                break;

            case ItemStatus.Locked_Unavailable:
                priceText.text = "LOCKED";
                buyButton.interactable = false;
                backgroundImage.color = Color.white;
                break;
        }
    }

    private void OnBuyClicked()
    {
        if (shopLoader != null)
        {
            // shopLoader.PurchaseItem handles both Buying (if locked) 
            // and Equipping (if owned/selectable)
            shopLoader.PurchaseItem(currentItem, this);
        }
    }
}