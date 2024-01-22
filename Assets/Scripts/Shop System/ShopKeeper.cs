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
        throw new System.NotImplementedException();
    }

    public void EndInteraction()
    {
        throw new System.NotImplementedException();
    }
}
