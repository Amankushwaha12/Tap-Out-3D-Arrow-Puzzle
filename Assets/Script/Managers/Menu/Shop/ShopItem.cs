using UnityEngine;
public enum ShopCategory { Head, Trail, Theme}

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Game/Shop Item")]
public class ShopItem : ScriptableObject
{
    public int id;              // Unique ID for sorting
    public string itemName;     // Display name
    public int price;           // Cost in game currency
    public Sprite icon;         // The image shown in the grid
    public ShopCategory category; // Category for sorting
    public Color color;
    public Color InnerColor, OutterColor;
    public float Influence;
}
