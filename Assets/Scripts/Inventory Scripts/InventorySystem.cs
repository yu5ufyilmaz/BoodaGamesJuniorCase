using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class InventorySystem
{
   [SerializeField] private List<InventorySlot> inventorySlots;
    [SerializeField] private int _gold;

    public int Gold => _gold;
   public List<InventorySlot> InventorySlots => inventorySlots;
   public int InventorySize => InventorySlots.Count;

   public UnityAction<InventorySlot> OnInventorySlotChanged;

   public InventorySystem(int size)
   {
        _gold = 0;
     CreateInventory(size);
   }

    public InventorySystem(int size,int gold)
    {
        _gold = gold;
        CreateInventory(size);
    }

    private void CreateInventory(int size)
    {
        inventorySlots = new List<InventorySlot>(size);

        for (int i = 0; i < size; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

   public bool AddToInventory(InventoryItemData itemToAdd, int amountToAdd)
   {
      if (ContainsItem(itemToAdd, out List<InventorySlot> invSlot)) // Item var mı yok mu kontrol eder
      {
         foreach (var slot in invSlot)
         {
            if (slot.EnoughRoomLeftInStack(amountToAdd))
            {
               slot.AddToStack(amountToAdd);
               OnInventorySlotChanged?.Invoke(slot);
               return true;
            }
         }
         
      }
      
      if (HasFreeSlot(out InventorySlot freeSlot)) // İlk müsait olan slotu alır
      {
         if (freeSlot.EnoughRoomLeftInStack(amountToAdd))
         {
            freeSlot.UpdateInventorySlot(itemToAdd,amountToAdd);
            OnInventorySlotChanged?.Invoke(freeSlot);
            return true;  
         }
      }
      return false;
   }

   public bool ContainsItem(InventoryItemData itemToAdd, out List<InventorySlot> invSlot)
   {
      invSlot = InventorySlots.Where(i => i.ItemData == itemToAdd).ToList();
      
      return invSlot == null ? false : true;
   }

   public bool HasFreeSlot(out InventorySlot freeSlot)
   {
      freeSlot = InventorySlots.FirstOrDefault(i => i.ItemData == null);
      return freeSlot == null ? false : true;
   }

   public bool CheckInventoryRemaining(Dictionary<InventoryItemData, int> shoppingCart)
   {
      var clonedSystem = new InventorySystem(this.InventorySize);

      for (int i = 0; i < InventorySize; i++)
      {
         clonedSystem.inventorySlots[i].AssignItem(this.inventorySlots[i].ItemData, this.InventorySlots[i].StackSize);

         foreach (var kvp in shoppingCart)
         {
            for (int j = 0; j < kvp.Value; j++)
            {
               if (!clonedSystem.AddToInventory(kvp.Key,1))
               {
                  return false;
               }
            }
         }

         
      }
      return true;
   }

   public void SpendGold(int basketTotal)
   {
      _gold -= basketTotal;
   }
}
