using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private ShopKeeperDisplay _shopKeeperDisplay;

    private void Awake()
    {
        _shopKeeperDisplay.gameObject.SetActive(false);
    }
    private void OnEnable()
   {
      ShopKeeper.OnShopWindowRequested += DisplayShopWindow;
   }

    private void OnDisable()
    {
      ShopKeeper.OnShopWindowRequested -= DisplayShopWindow;

    }

    private void DisplayShopWindow(ShopSystem shopSystem, PlayerInventoryHolder playerInventory)
    {
        _shopKeeperDisplay.gameObject.SetActive(true);
        _shopKeeperDisplay.DisplayShopWindow(shopSystem, playerInventory);
    }
}
