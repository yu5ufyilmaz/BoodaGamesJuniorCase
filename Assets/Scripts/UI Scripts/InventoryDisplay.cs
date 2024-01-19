using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class InventoryDisplay : MonoBehaviour
{
  [SerializeField] private MouseItemData mouseInventoryItem;
  protected InventorySystem inventorySystem;
  protected Dictionary<InventorySlot_UI, InventorySlot> slotDictionary;
  public InventorySystem InventorySystem => inventorySystem;
  public Dictionary<InventorySlot_UI, InventorySlot > SlotDictionary => slotDictionary;

  protected virtual void Start()
  {
    
  }

  public abstract void AssignSlot(InventorySystem invToDisplay);

  protected virtual void UpdateSlot(InventorySlot updatedSlot)
  {
    foreach (var slot in slotDictionary)
    {
      if (slot.Value == updatedSlot)
      {
        slot.Key.UpdateUISlot(updatedSlot); 
      }
    }
  }

  public void SlotClicked(InventorySlot_UI clickedUISlot)
  {
    if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData == null)
    {
      mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
      clickedUISlot.ClearSlot();
      return;
    }

    if (clickedUISlot.AssignedInventorySlot.ItemData == null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
    {
      clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
      clickedUISlot.UpdateUISlot();
      
      mouseInventoryItem.ClearSlot();  
    }


    if (clickedUISlot.AssignedInventorySlot.ItemData != null &&
        mouseInventoryItem.AssignedInventorySlot.ItemData != null)
    {
      if (clickedUISlot.AssignedInventorySlot.ItemData != mouseInventoryItem.AssignedInventorySlot.ItemData)
      {
        SwapSlots(clickedUISlot);
      }
    }  
  }

  private void SwapSlots(InventorySlot_UI clickedUISlot)
  {
    var clonedSlot = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData,
      mouseInventoryItem.AssignedInventorySlot.StackSize);
    mouseInventoryItem.ClearSlot();
    mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
    
    clickedUISlot.ClearSlot();
    clickedUISlot.AssignedInventorySlot.AssignItem(clonedSlot);
    clickedUISlot.UpdateUISlot();

  }
}
