using UnityEngine;

public enum ItemCategory
{
    PrimaryWeapon,
    Throwable,
    Collectible,
    Ammunition
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Item Information")]
    public string itemName;
    public Sprite itemIcon;

    [Header("Item Settings")]
    public ItemCategory category;
    public GameObject worldPrefab;

    [Header("Inventory")]
    public bool stackable;
    public int maxStackSize = 10;
}