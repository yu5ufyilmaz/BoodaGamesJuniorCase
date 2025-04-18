using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEditor.Rendering;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using Update = Unity.VisualScripting.Update;

public class MouseItemData : MonoBehaviour
{
   public Image ItemSprite;
   public TextMeshProUGUI ItemCount;
   public InventorySlot AssignedInventorySlot;
   
   [Header("Item Dropping Settings")]
   [SerializeField] private string defaultPrefabPath = "Prefabs/ItemPickUp"; // Default prefab yolu
   [SerializeField] private float dropDistance = 2f; // Oyuncunun önünde ne kadar mesafeye düşeceği
   [SerializeField] private float itemSpread = 0.5f; // Çoklu item atarken ne kadar yayılacağı
   [SerializeField] private float upwardForce = 2f; // Yukarı doğru uygulanan kuvvet
   [SerializeField] private float randomForce = 1f; // Rastgele uygulanan kuvvet
   
   [Tooltip("Item ID'lerini prefab yollarıyla eşleştiren sözlük. Özel prefab yolları için kullanın.")]
   [SerializeField] private List<ItemPrefabMapping> prefabMappings = new List<ItemPrefabMapping>();
   
   private Dictionary<int, string> itemPrefabDictionary = new Dictionary<int, string>();

   [System.Serializable]
   public class ItemPrefabMapping
   {
       public int itemID;
       public string prefabPath;
   }

   private void Awake()
   {
      ItemSprite.color = Color.clear;
      ItemCount.text = "";
      
      // Prefab mapping'leri sözlüğe doldur
      foreach (var mapping in prefabMappings)
      {
          if (!itemPrefabDictionary.ContainsKey(mapping.itemID))
          {
              itemPrefabDictionary.Add(mapping.itemID, mapping.prefabPath);
          }
      }
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
               // Get the stack size and item ID
               int stackSize = AssignedInventorySlot.StackSize;
               int itemID = AssignedInventorySlot.ItemData.ID;
               
               // Determine correct prefab path
               string prefabPath = defaultPrefabPath;
               if (itemPrefabDictionary.ContainsKey(itemID))
               {
                   prefabPath = itemPrefabDictionary[itemID];
               }
               
               // Determine drop position - in front of the player
               Vector3 dropPosition = player.transform.position + player.transform.forward * dropDistance;
               
               // Create item pickup
               GameObject itemPrefab = Resources.Load<GameObject>(prefabPath);
               if (itemPrefab != null)
               {
                   // Create stack of items with proper spread
                   for (int i = 0; i < stackSize; i++)
                   {
                       // Add a small random offset for multiple items
                       Vector3 offsetPosition = dropPosition;
                       if (i > 0)
                       {
                           offsetPosition += new Vector3(
                               UnityEngine.Random.Range(-itemSpread, itemSpread),
                               0,
                               UnityEngine.Random.Range(-itemSpread, itemSpread)
                           );
                       }
                       
                       GameObject newItemPickup = Instantiate(itemPrefab, offsetPosition, Quaternion.identity);
                       
                       // Try to get ItemPickUp component
                       ItemPickUp pickupComponent = newItemPickup.GetComponent<ItemPickUp>();
                       if (pickupComponent != null)
                       {
                           // If it has ItemPickUp component, set item data
                           pickupComponent.ItemData = AssignedInventorySlot.ItemData;
                       }
                       
                       // Add physics regardless of component type
                       Rigidbody rb = newItemPickup.GetComponent<Rigidbody>();
                       if (rb != null)
                       {
                           rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
                           
                           // Add a small random force for spread
                           if (i > 0)
                           {
                               rb.AddForce(
                                   new Vector3(
                                       UnityEngine.Random.Range(-randomForce, randomForce),
                                       0,
                                       UnityEngine.Random.Range(-randomForce, randomForce)
                                   ),
                                   ForceMode.Impulse
                               );
                           }
                       }
                   }
                   
                   Debug.Log($"Dropped {stackSize} {AssignedInventorySlot.ItemData.DisplayName} items using prefab: {prefabPath}");
               }
               else
               {
                   Debug.LogError($"Prefab not found at path: {prefabPath}");
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