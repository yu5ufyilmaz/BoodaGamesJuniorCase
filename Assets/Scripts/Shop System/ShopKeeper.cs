using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UniqueID))]
public class ShopKeeper : MonoBehaviour, IInteractable
{
    [SerializeField] private ShopItemList _shopItemHeld;
    [SerializeField] private ShopSystem _shopSystem;
    
    [Header("Shop Settings")]
    [SerializeField] private string shopkeeperName = "Shopkeeper";
    [SerializeField] private string shopGreeting = "Welcome! I'll buy your items.";

    public static UnityAction<ShopSystem, PlayerInventoryHolder> OnShopWindowRequested;

    private void Awake()
    {
        // Dükkan başlangıç ayarlarını yapılandır
        _shopSystem = new ShopSystem(_shopItemHeld.Items.Count, _shopItemHeld.MaxAllowedGold, _shopItemHeld.BuyMarkUp,
            _shopItemHeld.SellMarkUp);

        // Dükkana öğeleri ekle (bu öğeler artık oyuncuya gösterilmeyecek, 
        // bunun yerine oyuncunun topladığı öğeler gösterilecek)
        foreach (var item in _shopItemHeld.Items)
        {
            _shopSystem.AddToShop(item.ItemData, item.Amount);
        }
    }

    public UnityAction<IInteractable> OnInteractionComplete { get; set; }
    
    public void Interact(Interactor interactor, out bool interactSuccesful)
    {
        var playerInv = interactor.GetComponent<PlayerInventoryHolder>();

        if (playerInv != null)
        {
            // Oyuncu ile etkileşimde bulunduğunda dükkan arayüzünü göster
            OnShopWindowRequested?.Invoke(_shopSystem, playerInv);
            
            // Eğer istersen burada oyuncuya mesaj göster
            Debug.Log($"{shopkeeperName}: {shopGreeting}");
            
            interactSuccesful = true;
        }
        else
        {
            interactSuccesful = false;
            Debug.LogError("Player inventory not found");
        }
    }

    public void EndInteraction()
    {
        // Etkileşim sonlandığında yapılacak işlemler (gerekirse)
    }
}