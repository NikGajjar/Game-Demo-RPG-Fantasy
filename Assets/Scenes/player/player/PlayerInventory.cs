
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int inventorySize = 20;

    [Header("Inventory Items")]
    [SerializeField] private List<InventorySlot> slots = new();

    [Header("Selected Weapons")]
    [SerializeField] private WeaponData selectedSword;
    [SerializeField] private WeaponData selectedShield;
    [SerializeField] private WeaponData selectedBow;
    [SerializeField] private WeaponData selectedThrowable;

    [Header("Drop Settings")]
    [SerializeField] private Transform dropPoint;

    // Public access to selected weapons
    public WeaponData SelectedSword => selectedSword;
    public WeaponData SelectedShield => selectedShield;
    public WeaponData SelectedBow => selectedBow;
    public WeaponData SelectedThrowable => selectedThrowable;

    public WeaponData SelectedPrimaryWeapon =>
        selectedSword != null ? selectedSword :
        selectedShield != null ? selectedShield :
        selectedBow;

    public IReadOnlyList<InventorySlot> Slots => slots;

    private void Awake()
    {
        while (slots.Count < inventorySize)
        {
            slots.Add(new InventorySlot());
        }
    }

    // ADD ITEM 

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        int remainingCapacity = 0;

        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty)
            {
                remainingCapacity += item.stackable ? Mathf.Max(1, item.maxStackSize) : 1;
            }
            else if (item.stackable && slot.itemData == item)
            {
                remainingCapacity += Mathf.Max( 0,item.maxStackSize - slot.quantity );
            }
        }

        if (remainingCapacity < amount)
            return false;

        // Fill an existing stack first.
        if (item.stackable)
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot.IsEmpty || slot.itemData != item)
                    continue;

                int available = item.maxStackSize - slot.quantity;
                int added = Mathf.Min(available, amount);

                slot.quantity += added;
                amount -= added;

                if (amount <= 0)
                    break;
            }
        }

        // Use empty slots for remaining items.
        while (amount > 0)
        {
            InventorySlot empty = slots.Find(slot => slot.IsEmpty);

            if (empty == null)
                return false;

            int added = item.stackable ? Mathf.Min(item.maxStackSize, amount) : 1;

            empty.itemData = item;
            empty.quantity = added;

            amount -= added;
        }

        SetDefaultSelection(item);

        Debug.Log("Added item: " + item.itemName);
        return true;
    }

    //  DEFAULT SELECTION 

    private void SetDefaultSelection(ItemData item)
    {
        if (item is not WeaponData weapon)
            return;

        switch (weapon.weaponType)
        {
            case WeaponType.Sword:
                if (selectedSword == null)
                    selectedSword = weapon;
                break;

            case WeaponType.Shield:
                if (selectedShield == null)
                    selectedShield = weapon;
                break;

            case WeaponType.Bow:
                if (selectedBow == null)
                    selectedBow = weapon;
                break;

            case WeaponType.Snowball:
            case WeaponType.Rock:
                if (selectedThrowable == null)
                    selectedThrowable = weapon;
                break;
        }
    }

    // REMOVE ITEM 

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        if (GetItemCount(item) < amount)
            return false;

        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty || slot.itemData != item)
                continue;

            int removed = Mathf.Min(slot.quantity, amount);

            slot.quantity -= removed;
            amount -= removed;

            if (slot.quantity <= 0)
            {
                slot.itemData = null;
                slot.quantity = 0;
            }

            if (amount <= 0)
                break;
        }

        // Clear selected weapon if no copies remain.
        if (GetItemCount(item) == 0)
        {
            if (selectedSword == item)
                selectedSword = null;

            if (selectedShield == item)
                selectedShield = null;

            if (selectedBow == item)
                selectedBow = null;

            if (selectedThrowable == item)
                selectedThrowable = null;
        }

        return true;
    }

    // GET ITEM COUNT 

    public int GetItemCount(ItemData item)
    {
        if (item == null)
            return 0;

        int count = 0;

        foreach (InventorySlot slot in slots)
        {
            if (!slot.IsEmpty && slot.itemData == item)
                count += slot.quantity;
        }

        return count;
    }

    //  WEAPON SELECTION 

    public void SelectSword()
    {
        if (selectedSword != null && GetItemCount(selectedSword) > 0)
        {
            Debug.Log("Selected Sword");
        }
    }

    public void SelectShield()
    {
        if (selectedShield != null && GetItemCount(selectedShield) > 0)
        {
            Debug.Log("Selected Shield");
        }
    }

    public void SelectBow()
    {
        if (selectedBow != null && GetItemCount(selectedBow) > 0)
        {
            Debug.Log("Selected Bow");
        }
    }

    public void SelectThrowable()
    {
        if (selectedThrowable != null && GetItemCount(selectedThrowable) > 0)
        {
            Debug.Log("Selected Throwable");
        }
    }

    // Compatibility method for older CombatController code.
    public void SelectPrimaryWeapon(WeaponData weapon)
    {
        if (weapon == null || GetItemCount(weapon) <= 0)
            return;

        switch (weapon.weaponType)
        {
            case WeaponType.Sword:
                selectedSword = weapon;
                break;

            case WeaponType.Shield:
                selectedShield = weapon;
                break;

            case WeaponType.Bow:
                selectedBow = weapon;
                break;
        }
    }

    public void SelectThrowable(WeaponData weapon)
    {
        if (weapon == null || weapon.weaponType != WeaponType.Snowball &&weapon.weaponType != WeaponType.Rock || GetItemCount(weapon) <= 0)
            return;

        selectedThrowable = weapon;
    }

    //  DROP ITEM 

    public bool DropItem(ItemData item, int amount = 1)
    {
        if (item == null || item.worldPrefab == null || amount <= 0)
            return false;

        if (GetItemCount(item) < amount)
            return false;

        Vector3 position = dropPoint != null ? dropPoint.position : transform.position + transform.forward * 1.5f;

        if (!RemoveItem(item, amount))
            return false;

        GameObject dropped = Instantiate( item.worldPrefab, position, Quaternion.identity );

        ItemPickup pickup = dropped.GetComponent<ItemPickup>();

        if (pickup != null)
            pickup.SetItem(item, amount);

        Debug.Log("Dropped item: " + item.itemName);

        return true;
    }
}
