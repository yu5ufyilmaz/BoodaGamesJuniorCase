using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UniqueID))]
public class ShopKeeper : MonoBehaviour,IInteractable
{
    [SerializeField] private ShopItemList _shopItemHeld;
    [SerializeField] private ShopSystem _shopSystem;

    public static UnityAction<ShopSystem, PlayerInventoryHolder> OnShopWindowRequested;

    private void Awake()
    {
        _shopSystem = new ShopSystem(_shopItemHeld.Items.Count, _shopItemHeld.MaxAllowedGold, _shopItemHeld.BuyMarkUp,
            _shopItemHeld.SellMarkUp);


        foreach (var item in _shopItemHeld.Items)
        {
            Debug.Log($"{item.ItemData.DisplayName} : {item.Amount}");
            _shopSystem.AddToShop(item.ItemData, item.Amount);
        }
    }
    

    public UnityAction<IInteractable> OnInteractionComplete { get; set; }
    public void Interact(Interactor interactor, out bool interactSuccesful)
    {
        var playerInv = interactor.GetComponent<PlayerInventoryHolder>();

        if (playerInv != null)
        {
            OnShopWindowRequested?.Invoke(_shopSystem,playerInv);
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
        
    }
}
