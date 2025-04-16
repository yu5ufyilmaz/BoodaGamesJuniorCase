using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(UniqueID))]
public class ItemPickUp : MonoBehaviour, IInteractable
{
    public float PickUpRadius = 1f;
    public InventoryItemData ItemData;
    
    [Header("Feedback")]
    [SerializeField] private AudioSource pickupSound;
    [SerializeField] private GameObject pickupEffect;

    private SphereCollider myCollider;
    [SerializeField] private ItemPickUpSaveData itemSaveData;

    private string id;
    private GameManager gameManager;

    public UnityAction<IInteractable> OnInteractionComplete { get; set; }

    private void Awake()
    {
        id = GetComponent<UniqueID>().ID;
        SaveLoad.OnLoadGame += LoadGame;
        itemSaveData = new ItemPickUpSaveData(ItemData, transform.position, transform.rotation);
        
        myCollider = GetComponent<SphereCollider>();
        myCollider.isTrigger = true;
        myCollider.radius = PickUpRadius;
        
        // Find the GameManager
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        SaveGameManager.data.activeItems.Add(id, itemSaveData);
    }

    private void LoadGame(SaveData data)
    {
        if (data.collectedItems.Contains(id)) 
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (SaveGameManager.data != null && SaveGameManager.data.activeItems != null && 
            SaveGameManager.data.activeItems.ContainsKey(id))
        {
            SaveGameManager.data.activeItems.Remove(id);
        }

        SaveLoad.OnLoadGame -= LoadGame;
    }

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        var inventory = interactor.GetComponent<PlayerInventoryHolder>();

        if (!inventory)
        {
            Debug.LogWarning($"No PlayerInventoryHolder component found on {interactor.gameObject.name}");
            interactSuccessful = false;
            return;
        }

        if (inventory.AddToInventory(ItemData, 1))
        {
            // Play pickup effects if available
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound.clip, transform.position);
            }
        
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }
        
            // Check if SaveGameManager.data and collectedItems are not null before adding
            if (SaveGameManager.data != null && SaveGameManager.data.collectedItems != null)
            {
                SaveGameManager.data.collectedItems.Add(id);
            }
            else
            {
                Debug.LogWarning("SaveGameManager.data or collectedItems is null. Item ID not saved.");
            }
        
            interactSuccessful = true;
        
            // Destroy the object after interaction
            Destroy(this.gameObject);
        }
        else
        {
            interactSuccessful = false;
        }
    }

    public void EndInteraction()
    {
        // ItemPickup için bu metodu uygulamaya gerek yok
    }



}

[System.Serializable]
public struct ItemPickUpSaveData
{
    public InventoryItemData ItemData;
    public Vector3 Position;
    public Quaternion Rotation;

    public ItemPickUpSaveData(InventoryItemData _data, Vector3 _position, Quaternion _rotation)
    {
        ItemData = _data;
        Position = _position;
        Rotation = _rotation;
    }
}