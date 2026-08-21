using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using TMPro; // Ensure DOTween is imported

public class ShopElementLoader : MonoBehaviour
{
    public RectTransform leftTab, middleTab, rightTab;
    [Header("Tab Positions")]
    // public Vector2[] leftTabPositions;  // Set index 0, 1, 2 in Inspector
    // public Vector2[] middleTabPositions; 
    // public Vector2[] rightTabPositions;
    [Header("Tab Text Colors")]
    public Color selectedColor = Color.white;
    public Color dullColor = new Color(0.7f, 0.7f, 0.7f, 0.8f); // Light gray/dull

    public TextMeshProUGUI[] tabTexts;
    
    public GameObject itemButtonPrefab;
    public Transform gridParent;
    public CanvasGroup gridCanvasGroup;
    public CoinDisplay coinDisplay;

    public MeshRenderer domeMeshRenderer;
    public ShopItem currentTheme; 
    
    private List<ShopItem> allShopItems = new List<ShopItem>();

    void Start()
    {
        // REMOVED PlayerPrefs.DeleteAll(); - Do not delete your JSON save data!
        
        allShopItems = Resources.LoadAll<ShopItem>("ShopItem").ToList();
        
        // This now handles initialization using the JSON system
        InitializeDefaultShopItems(allShopItems);
        
        LoadCategory(ShopCategory.Head);
    }

    public void LoadCategory(ShopCategory category)
    {
        int index = 0;
        switch (category)
        {
            case ShopCategory.Head:  index = 0; break;
            case ShopCategory.Trail: index = 1; break;
            case ShopCategory.Theme: index = 2; break;
        }

        for (int i = 0; i < tabTexts.Length; i++)
        {
            bool isSelected = (i == index);
            
            // 1. Smooth Color Tween
            tabTexts[i].DOColor(isSelected ? selectedColor : dullColor, 0.3f);
            
            // 2. Smooth Scale Tween for the "Bold" pop effect
            float targetScale = isSelected ? 1.1f : 1.0f;
            tabTexts[i].transform.DOScale(targetScale, 0.3f).SetEase(Ease.OutQuad);
        }

        // Now you only need to call AnimateBackgroundTabs and SwitchCategoryRoutine here
        AnimateBackgroundTabs(index);
        StartCoroutine(SwitchCategoryRoutine(category));
    }
    private void AnimateBackgroundTabs(int index)
    {
        float duration = 0.4f;
        // OutQuad is smooth, fast, and stays within your boundaries (no bounce)
        Ease easeType = Ease.OutQuad; 

        // Apply the movement with the non-bouncy ease
        // leftTab.DOAnchorPos(leftTabPositions[index], duration).SetEase(easeType);
        // middleTab.DOAnchorPos(middleTabPositions[index], duration).SetEase(easeType);
        // rightTab.DOAnchorPos(rightTabPositions[index], duration).SetEase(easeType);
    }



    private System.Collections.IEnumerator SwitchCategoryRoutine(ShopCategory category)
    {
        // 1. Fade Out
        float duration = 0.3f;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            gridCanvasGroup.alpha = Mathf.Lerp(1, 0, t / duration);
            yield return null;
        }

        // 2. Clear Grid
        foreach (Transform child in gridParent) Destroy(child.gameObject);
        // 3. Populate Grid
        var filteredItems = allShopItems.Where(item => item.category == category).OrderBy(i => i.id);
        foreach (var item in filteredItems)
        {
            GameObject btnObj = Instantiate(itemButtonPrefab, gridParent);
            ShopButtonUI btnUI = btnObj.GetComponent<ShopButtonUI>();
            btnUI.buyButton.onClick.RemoveAllListeners();
            // Pass 'this' so the button can communicate back to the loader
            btnUI.Setup(item, this);
            btnUI.buyButton.onClick.AddListener(() =>
            {
                // AudioManager.Instance.PlayButtonClick();
                ApplyTheme();
            });
        }
        // 4. Fade In
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            gridCanvasGroup.alpha = Mathf.Lerp(0, 1, t / duration);
            yield return null;
        }
    }

    public void PurchaseItem(ShopItem item, ShopButtonUI buttonUI)
    {
        ItemStatus status = ShopData.GetItemStatus(item);

        if (status == ItemStatus.Locked_Buyable)
        {
            if (Wallet.TryPurchase(item.price))
            {
                coinDisplay.CoinTextUpdate();
                ShopData.SetItemState(item, 1);
                RefreshAllButtons();
            }
        }
        else if (status == ItemStatus.Unlocked_Owned)
        {
            // Successfully switches state to "Equipped" and saves to JSON
            ShopData.EquipItem(item, item.category);
            RefreshAllButtons();
        }
    }

    private void RefreshAllButtons()
    {
        foreach (Transform child in gridParent)
        {
            ShopButtonUI btn = child.GetComponent<ShopButtonUI>();
            if (btn != null)
            {
                btn.UpdateVisuals();
            }
        }
    }

    public void HeadPanel()  => LoadCategory(ShopCategory.Head);
    public void TrailPanel() => LoadCategory(ShopCategory.Trail);
    public void ThemePanel() => LoadCategory(ShopCategory.Theme);

    public static void InitializeDefaultShopItems(List<ShopItem> allItems)
    {
        foreach (ShopCategory category in System.Enum.GetValues(typeof(ShopCategory)))
        {
            // If this category is empty in our JSON, set the default
            if (!Wallet.data.activeItems.ContainsKey(category.ToString()))
            {
                ShopItem defaultItem = allItems
                    .Where(i => i.category == category)
                    .OrderBy(i => i.id)
                    .FirstOrDefault();

                if (defaultItem != null)
                {
                    if (!Wallet.data.ownedItemIds.Contains(defaultItem.id))
                    {
                        Wallet.data.ownedItemIds.Add(defaultItem.id);
                    }

                    Wallet.data.activeItems[category.ToString()] = defaultItem.id;
                    Wallet.Save(); // Persists to wallet.json
                    
                    Debug.Log($"Default {category} set to {defaultItem.name}");
                }
            }
        }
    }

    public void ApplyTheme()
    {
        Debug.Log("Apply Theme Called.");
        ShopItem[] allItems = Resources.LoadAll<ShopItem>("ShopItem");
        int themeID = ShopData.GetActiveItemID(ShopCategory.Theme);
        
        currentTheme = System.Array.Find(allItems, i => i.id == themeID && i.category == ShopCategory.Theme);
        Material mat = domeMeshRenderer.material;

        // 3. Apply the properties defined in the Inspector
        // These names match the properties you provided
        mat.SetColor("_InnerColor", currentTheme.InnerColor);
        mat.SetColor("_OutterColor", currentTheme.OutterColor);
        mat.SetFloat("_Influence", currentTheme.Influence);
    }
}