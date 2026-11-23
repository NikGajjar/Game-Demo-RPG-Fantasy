using System;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public ItemData itemData;
    public int quantity;

    public bool IsEmpty => itemData == null || quantity <= 0;
}