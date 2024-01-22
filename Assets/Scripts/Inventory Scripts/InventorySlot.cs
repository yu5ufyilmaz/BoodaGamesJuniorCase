using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot : ItemSlot
{
    public InventorySlot(InventoryItemData source, int amount)
    {
        itemData = source;
        _itemID = itemData.ID;
        stackSize = amount;
    }

    public InventorySlot()
    {
        ClearSlot();
    }

    public void UpdateInventorySlot(InventoryItemData data, int amount)
    {
        itemData = data;
        _itemID = itemData.ID;
        stackSize = amount;
    }

    public bool EnoughRoomLeftInStack(int amountToAdd, out int amountRemaining)
    {
        amountRemaining = ItemData.MaxStackSize - stackSize;
        return EnoughRoomLeftInStack(amountToAdd);
    }

    public bool EnoughRoomLeftInStack(int amountToAdd)
    {
        if (itemData == null || itemData != null && stackSize + amountToAdd <= itemData.MaxStackSize) return true;
        else
            return false;
    }


    public void RemoveFromStack(int amount)
    {
        stackSize -= amount;
    }

    public bool SplitStack(out InventorySlot splitStack)
    {
        if (stackSize <= 1)
        {
            splitStack = null;
            return false;
        }

        int halfStack = Mathf.RoundToInt(stackSize / 2);
        RemoveFromStack(halfStack);

        splitStack = new InventorySlot(itemData, halfStack);
        return true;
    }

}
