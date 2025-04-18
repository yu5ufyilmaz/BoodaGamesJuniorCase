using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class DynamicInventoryDisplay : InventoryDisplay
{
    [SerializeField] protected InventorySlot_UI slotPrefab;
    
    public override void AssignSlot(InventorySystem invToDisplay,int offset)
    {
        ClearSlots();

        slotDictionary = new Dictionary<InventorySlot_UI, InventorySlot>();
        if (invToDisplay == null)
        {
            return;
        }

        for (int i = offset; i < invToDisplay.InventorySize; i++)
        {
            var uiSlot = Instantiate(slotPrefab, transform);
            slotDictionary.Add(uiSlot,invToDisplay.InventorySlots[i]);
            uiSlot.Init(invToDisplay.InventorySlots[i]);
            uiSlot.UpdateUISlot();
        }
    }

    public void RefreshDynamicInventory(InventorySystem invToDisplay,int offset)
    {
        ClearSlots();
        inventorySystem = invToDisplay;
        if (inventorySystem != null)
        {
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
        }

       
        AssignSlot(invToDisplay,offset);
    }

    private void ClearSlots()
    {
        foreach (var item in transform.Cast<Transform>())
        {
            Destroy(item.gameObject);
        }

        if (slotDictionary != null)
        {
            slotDictionary.Clear();
        }
    }

    private void OnDisable()
    {
        if (inventorySystem != null)
        {
            inventorySystem.OnInventorySlotChanged -= UpdateSlot;
        }
    }
}
