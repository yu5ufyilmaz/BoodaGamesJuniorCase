using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticInventoryDisplay : InventoryDisplay
{
    [SerializeField] private InventoryHolder inventoryHolder;
    [SerializeField] private InventorySlot_UI[] slots;

    private void OnEnable()
    {
        PlayerInventoryHolder.OnPlayerInventoryChanged += RefreshStaticDisplay;
    }

    private void OnDisable()
    {
        PlayerInventoryHolder.OnPlayerInventoryChanged -= RefreshStaticDisplay;

    }


    private void RefreshStaticDisplay()
    {
        if (inventoryHolder != null)
        {
            inventorySystem = inventoryHolder.PrimaryInventorySystem;
            if (inventorySystem != null)
            {
                inventorySystem.OnInventorySlotChanged += UpdateSlot;
                AssignSlot(inventorySystem, 0);
            }
        }
        else
        {
            Debug.LogWarning($"No inventory assigned to {this.gameObject}");
            // Burada erken dönüş ekliyoruz ki hata olmasın
            return;
        }
    }

    protected override void Start()
    {
        base.Start();
        RefreshStaticDisplay();
       
    }
    public override void AssignSlot(InventorySystem invToDisplay, int offset)
    {
        // inventorySystem null olabilir, kontrol ekleyelim
        if (inventorySystem == null) return;
    
        slotDictionary = new Dictionary<InventorySlot_UI, InventorySlot>();
    
        // slots veya inventorySystem.InventorySlots null olabilir, kontrol ekleyelim
        if (slots == null || inventorySystem.InventorySlots == null) return;
    
        for (int i = 0; i < inventoryHolder.Offset && i < slots.Length && i < inventorySystem.InventorySlots.Count; i++)
        {
            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Init(inventorySystem.InventorySlots[i]);
        }
    }
}
