
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Information")]
    [SerializeField] private ItemData itemData;
    [SerializeField] private int quantity = 1;

    public ItemData Item => itemData;
    public int Quantity => quantity;

    public void SetItem(ItemData item, int amount)
    {
        itemData = item;
        quantity = amount;
    }

    public bool TryPickup(PlayerInventory inventory)
    {
        if (inventory == null || itemData == null)
            return false;

        if (!inventory.AddItem(itemData, quantity))
            return false;

        Destroy(gameObject);
        return true;
    }
}
