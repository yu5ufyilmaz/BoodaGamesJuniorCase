using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using Update = Unity.VisualScripting.Update;

public class MouseItemData : MonoBehaviour
{
   public Image ItemSprite;
   public TextMeshProUGUI ItemCount;
   public InventorySlot AssignedInventorySlot;

   private void Awake()
   {
      ItemSprite.color = Color.clear;
      ItemCount.text = "";
      
   }

   public void UpdateMouseSlot(InventorySlot invSlot)
   {
      AssignedInventorySlot.AssignItem(invSlot);
      ItemSprite.sprite = invSlot.ItemData.Icon;
      ItemCount.text = invSlot.StackSize.ToString();
      ItemSprite.color = Color.white;

   }

   private void Update()
   {
      if (AssignedInventorySlot.ItemData != null)
      {
         transform.position = Mouse.current.position.ReadValue();
         if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUIObject())
         {
            ClearSlot();
         }
      }
   }

   public void ClearSlot()
   {
      AssignedInventorySlot.ClearSlot();
      ItemCount.text = "";
      ItemSprite.color = Color.clear;
      ItemSprite.sprite = null;
   }
   public static bool IsPointerOverUIObject()
   {
      PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
      eventDataCurrentPosition.position = Mouse.current.position.ReadValue();
      List<RaycastResult> raycastResults = new List<RaycastResult>();
      EventSystem.current.RaycastAll(eventDataCurrentPosition,raycastResults);
      return raycastResults.Count > 0;
   }
}