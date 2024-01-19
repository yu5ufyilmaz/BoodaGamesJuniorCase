using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class InventoryUIController : MonoBehaviour
{
   [FormerlySerializedAs("inventoryPanel")] public DynamicInventoryDisplay chestPanel;
   public DynamicInventoryDisplay playerBackpackPanel;

   private void Awake()
   {
      chestPanel.gameObject.SetActive(false);
      playerBackpackPanel.gameObject.SetActive(false);

   }

   private void OnEnable()
   {
      InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayInventory;
      PlayerInventoryHolder.OnPlayerBackpackDisplayRequested += DisplayPlayerBackpack;
   }

   private void OnDisable()
   {
      InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayInventory;
      PlayerInventoryHolder.OnPlayerBackpackDisplayRequested -= DisplayPlayerBackpack;

      
   }

   private void Update()
   {
      if (chestPanel.gameObject.activeInHierarchy && Keyboard.current.escapeKey.wasPressedThisFrame)
      {
         chestPanel.gameObject.SetActive(false);
      }
      
      if (playerBackpackPanel.gameObject.activeInHierarchy && Keyboard.current.escapeKey.wasPressedThisFrame)
      {
         playerBackpackPanel.gameObject.SetActive(false);
      }
   }

   void  DisplayInventory(InventorySystem invToDisplay)
   {
      chestPanel.gameObject.SetActive(true);
      chestPanel.RefreshDynamicInventory(invToDisplay);
   }
   
   void  DisplayPlayerBackpack(InventorySystem invToDisplay)
   {
      playerBackpackPanel.gameObject.SetActive(true);
      playerBackpackPanel.RefreshDynamicInventory(invToDisplay);
   }
}
