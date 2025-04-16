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
            // Create a physical item in the world
            DropItemToWorld();
            
            // Clear the mouse slot
            ClearSlot();
        }
    }
}

private void DropItemToWorld()
{
    if (AssignedInventorySlot.ItemData != null)
    {
        // Find the player to determine drop position
        PlayerInventoryHolder player = FindObjectOfType<PlayerInventoryHolder>();
        if (player != null)
        {
            // Determine drop position - in front of the player
            Vector3 dropPosition = player.transform.position + player.transform.forward * 2f;
            
            // Create item pickup
            GameObject itemPickupPrefab = Resources.Load<GameObject>("Prefabs/ItemPickUp");
            if (itemPickupPrefab != null)
            {
                GameObject newItemPickup = Instantiate(itemPickupPrefab, dropPosition, Quaternion.identity);
                ItemPickUp pickupComponent = newItemPickup.GetComponent<ItemPickUp>();
                
                if (pickupComponent != null)
                {
                    // Set the item data
                    pickupComponent.ItemData = AssignedInventorySlot.ItemData;
                    
                    // Add a small upward force for a natural dropping effect
                    Rigidbody rb = newItemPickup.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
                    }
                    
                    Debug.Log($"Dropped item: {AssignedInventorySlot.ItemData.DisplayName}");
                }
                else
                {
                    Debug.LogError("ItemPickUp component not found on the prefab!");
                }
            }
            else
            {
                Debug.LogError("ItemPickUp prefab not found in Resources/Prefabs folder!");
            }
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
