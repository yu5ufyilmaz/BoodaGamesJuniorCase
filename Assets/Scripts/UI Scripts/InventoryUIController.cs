using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class InventoryUIController : MonoBehaviour
{
   [FormerlySerializedAs("chestPanel")] public DynamicInventoryDisplay inventoryPanel;
   public DynamicInventoryDisplay playerBackpackPanel;

   private void Awake()
   {
      inventoryPanel.gameObject.SetActive(false);
      playerBackpackPanel.gameObject.SetActive(false);

   }

   private void OnEnable()
   {
      InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayInventory;
      PlayerInventoryHolder.OnPlayerInventoryDisplayRequested += DisplayPlayerInventory;
   }

   private void OnDisable()
   {
      InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayInventory;
      PlayerInventoryHolder.OnPlayerInventoryDisplayRequested -= DisplayPlayerInventory;

   }

   private void Update()
   {
      if (inventoryPanel.gameObject.activeInHierarchy && Keyboard.current.escapeKey.wasPressedThisFrame)
      {
         inventoryPanel.gameObject.SetActive(false);
      }
      
      if (playerBackpackPanel.gameObject.activeInHierarchy && Keyboard.current.escapeKey.wasPressedThisFrame)
      {
         playerBackpackPanel.gameObject.SetActive(false);
      }
   }

   void  DisplayInventory(InventorySystem invToDisplay,int offset)
   {
      inventoryPanel.gameObject.SetActive(true);
      inventoryPanel.RefreshDynamicInventory(invToDisplay,offset);
   }
   private void DisplayPlayerInventory(InventorySystem invToDisplay, int offset)
   {
      playerBackpackPanel.gameObject.SetActive(true);
      playerBackpackPanel.RefreshDynamicInventory(invToDisplay, offset);
    
      // Debug ekleyelim
      Debug.Log($"Oyuncu envanteri gösteriliyor. Item sayısı: {invToDisplay.InventorySize}");
      foreach (var slot in invToDisplay.InventorySlots)
      {
         if (slot.ItemData != null)
            Debug.Log($"Item: {slot.ItemData.DisplayName}, Adet: {slot.StackSize}");
      }
   }
}
