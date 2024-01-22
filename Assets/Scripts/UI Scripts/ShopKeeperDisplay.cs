using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopKeeperDisplay : MonoBehaviour
{
   [SerializeField] private ShopSlotUI _shopSlotPrefab;
   [SerializeField] private ShoppingCartItemUI _shoppingCartItemPrefab;

   [SerializeField] private Button _buyTab;
   [SerializeField] private Button _sellTab;

   [Header("Shhopping Cart")] 
  [SerializeField] private TextMeshProUGUI _basketTotalText;

   [SerializeField] private TextMeshProUGUI _playerGoldText;
   [SerializeField] private TextMeshProUGUI _shopGoldText;
   [SerializeField] private Button _buyButton;
   [SerializeField] private TextMeshProUGUI _buyButtonText;

   [Header("Item Preview Section")] 
  [SerializeField] private Image _itemPreviewSprite;

   [SerializeField] private TextMeshProUGUI _itemPreviewName;
   [SerializeField] private TextMeshProUGUI _itemPreviewDescription;

   [SerializeField] private GameObject _itemListContentPanel;
   [SerializeField] private GameObject _shoppingCartContentPanel;

    private int _basketTotal;

    private ShopSystem _shopSystem;
    private PlayerInventoryHolder _playerInventoryHolder;

    private Dictionary<InventoryItemData, int> _shoppingCart = new Dictionary<InventoryItemData, int>();
    private Dictionary<InventoryItemData, ShoppingCartItemUI> _shoppingCartUI = new Dictionary<InventoryItemData, ShoppingCartItemUI>();

    public void DisplayShopWindow(ShopSystem shopSystem,PlayerInventoryHolder playerInventoryHolder)
    {
        _shopSystem = shopSystem;
        _playerInventoryHolder = playerInventoryHolder;

        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        ClearSlots();

        _basketTotalText.enabled = false;
        _buyButton.gameObject.SetActive(false);
        _basketTotal = 0;
        _playerGoldText.text = $"Player Gold: {_playerInventoryHolder.PrimaryInventorySystem.Gold}";
        _shopGoldText.text = $"Shop Gold: {_shopSystem.AvailableGold}";

        DisplayShopInventory();
    }
    

    private void ClearSlots()
    {
        _shoppingCart = new Dictionary<InventoryItemData, int>();
        _shoppingCartUI = new Dictionary<InventoryItemData, ShoppingCartItemUI>();

        foreach (var item in _itemListContentPanel.transform.Cast<Transform>())
        {
            Destroy(item.gameObject);
        }

        foreach (var item in _shoppingCartContentPanel.transform.Cast<Transform>())
        {
            Destroy(item.gameObject);
        }

        
    }
    private void DisplayShopInventory()
    {
        foreach (var item in _shopSystem.ShopInventory)
        {
            if (item.ItemData == null) { continue; ; }

            var shopSlot = Instantiate(_shopSlotPrefab, _itemListContentPanel.transform);
            shopSlot.Init(item,_shopSystem.BuyMarkUp);
        }
    }

    private void DisplayPlayerInventory()
    {

    }
}
