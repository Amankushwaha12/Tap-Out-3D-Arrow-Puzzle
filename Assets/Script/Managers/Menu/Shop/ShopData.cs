using UnityEngine;

public enum ItemStatus { Locked_Unavailable, Locked_Buyable, Unlocked_Owned, Unlocked_Equipped }

public static class ShopData
{
    public static ItemStatus GetItemStatus(ShopItem item)
    {
        // 1. First, check if it's the currently EQUIPPED item
        int activeID = GetActiveItemID(item.category);
        if (activeID == item.id) 
            return ItemStatus.Unlocked_Equipped;

        // 2. If not equipped, check if we own it
        if (Wallet.data.ownedItemIds.Contains(item.id)) 
            return ItemStatus.Unlocked_Owned;
            
        // 3. Otherwise, it's buyable
        return ItemStatus.Locked_Buyable;
    }

    public static void SetItemState(ShopItem item, int status)
    {
        // Only add if they don't already own it
        if (status == 1 && !Wallet.data.ownedItemIds.Contains(item.id))
        {
            Wallet.data.ownedItemIds.Add(item.id);
            Wallet.Save();
        }
    }

    public static void EquipItem(ShopItem item, ShopCategory category)
    {
        string key = category.ToString();
        Debug.Log($"Equipping {item.name} into Category Key: {key}");
        Wallet.data.activeItems[key] = item.id;
        Wallet.Save(); 
    }

    public static int GetActiveItemID(ShopCategory category)
    {
        string key = category.ToString();
        // DEBUG THIS LINE
        foreach(var k in Wallet.data.activeItems.Keys) { Debug.Log($"Existing Key: {k}" + Wallet.data.activeItems[key]); }

        if (Wallet.data.activeItems.ContainsKey(key))
            return Wallet.data.activeItems[key];

        return -1;
    }
}