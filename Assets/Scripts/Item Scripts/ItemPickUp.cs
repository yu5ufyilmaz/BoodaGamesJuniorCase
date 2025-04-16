using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(UniqueID))]
public class ItemPickUp : MonoBehaviour
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
        if (SaveGameManager.data.activeItems.ContainsKey(id))
        {
            SaveGameManager.data.activeItems.Remove(id);
        }

        SaveLoad.OnLoadGame -= LoadGame;
    }

    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.transform.GetComponent<PlayerInventoryHolder>();

        if (!inventory)
        {
            Debug.LogWarning($"No PlayerInventoryHolder component found on {other.gameObject.name}");
            return;
        }

        if (inventory.AddToInventory(ItemData, 1))
        {
            // Add score based on item's value when picked up
            if (gameManager != null)
            {
                // We could add immediate feedback here, but final scoring happens at the end
                // If you want visual feedback, you can show a floating text
                
                // Play pickup effects if available
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound.clip, transform.position);
                }
                
                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }
            }
            
            SaveGameManager.data.collectedItems.Add(id);
            Destroy(this.gameObject);
        }
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