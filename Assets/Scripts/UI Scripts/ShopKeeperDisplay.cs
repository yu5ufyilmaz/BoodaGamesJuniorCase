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
    
    private bool _isSelling;

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
        if (_buyButton != null)
        {
            _buyButtonText.text = _isSelling? "Sell Items" : "Buy Items";
            _buyButton.onClick.RemoveAllListeners();
            if (_isSelling)
            {
                _buyButton.onClick.AddListener(SellItems);
            }
            else
            {
                _buyButton.onClick.AddListener(BuyItems);
            }
        }
        
        ClearSlots();

        _basketTotalText.enabled = false;
        _buyButton.gameObject.SetActive(false);
        _basketTotal = 0;
        _playerGoldText.text = $"Player Gold: {_playerInventoryHolder.PrimaryInventorySystem.Gold}";
        _shopGoldText.text = $"Shop Gold: {_shopSystem.AvailableGold}";

        DisplayShopInventory();
    }

    private void BuyItems()
    {
        if (_playerInventoryHolder.PrimaryInventorySystem.Gold < _basketTotal)
        {
            return;
        }

        if (!_playerInventoryHolder.PrimaryInventorySystem.CheckInventoryRemaining(_shoppingCart))
        {
            return;
        }

        foreach (var kvp in _shoppingCart)
        {
            _shopSystem.PurchaseItem(kvp.Key, kvp.Value);

            for (int i = 0; i < kvp.Value; i++)
            {
                _playerInventoryHolder.PrimaryInventorySystem.AddToInventory(kvp.Key, 1);
            }
        }

        _shopSystem.GainGold(_basketTotal);
        _playerInventoryHolder.PrimaryInventorySystem.SpendGold(_basketTotal);
        
        RefreshDisplay();
    }

    private void SellItems()
    {
        
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
    
    public void RemoveItemFromCart(ShopSlotUI shopSlotUI)
    {
        var data = shopSlotUI.AssignedItemSlot.ItemData;
        
        var price = GetModifiedPrice(data, 1, shopSlotUI.MarkUp);

        if (_shoppingCart.ContainsKey(data))
        {
            _shoppingCart[data]--;
            var newString = $"{data.DisplayName} ({price}G) x{_shoppingCart[data]}";
            _shoppingCartUI[data].SetItemText(newString);

            if (_shoppingCart[data] <= 0)
            {
                _shoppingCart.Remove(data);
                var tempObj = _shoppingCartUI[data].gameObject;
                _shoppingCartUI.Remove(data);
                Destroy(tempObj);
            }
        }

        _basketTotal -= price;
        _basketTotalText.text = $"Total: {_basketTotal}G";

        if (_basketTotal <= 0 && _basketTotalText.IsActive())
        {
            _basketTotalText.enabled = false;
            _buyButton.gameObject.SetActive(false);
            ClearItemPreview();
            return;
        }
        
        CheckCartVsAvailableGold();
    }

    private void ClearItemPreview()
    {
      
    }

    public void AddItemRoCart(ShopSlotUI shopSlotUI)
    {
        var data = shopSlotUI.AssignedItemSlot.ItemData;

        UpdateItemPreview(shopSlotUI);
        var price = GetModifiedPrice(data, 1, shopSlotUI.MarkUp);
       

        if (_shoppingCart.ContainsKey(data))
        {
            _shoppingCart[data]++;
            var newString = $"{data.DisplayName} ({price}G) x{_shoppingCart[data]}";
            _shoppingCartUI[data].SetItemText(newString);
           
        }
        else
        {
            var newString = $"{data.DisplayName} ({price}G) x1";
            _shoppingCart.Add(data,1);
            var shoppingCartTextObj = Instantiate(_shoppingCartItemPrefab, _shoppingCartContentPanel.transform);
            shoppingCartTextObj.SetItemText(newString);
            _shoppingCartUI.Add(data,shoppingCartTextObj);
        }

        _basketTotal += price;
        _basketTotalText.text = $"Total: {_basketTotal}G";

        if (_basketTotal >0 && !_basketTotalText.IsActive())
        {
            _basketTotalText.enabled = true;
            _buyButton.gameObject.SetActive(true);
        }

        CheckCartVsAvailableGold();
    }

    
    private void CheckCartVsAvailableGold()
    {
        var goldToCheck = _isSelling ? _shopSystem.AvailableGold : _playerInventoryHolder.PrimaryInventorySystem.Gold;
        _basketTotalText.color = _basketTotal > goldToCheck ? Color.red : Color.green;

        if (_isSelling || _playerInventoryHolder.PrimaryInventorySystem.CheckInventoryRemaining(_shoppingCart))
        {
            return;
        }

        _basketTotalText.text = "Not enough room in inventory.";
        _basketTotalText.color = Color.red;
    }

    public static int GetModifiedPrice(InventoryItemData data, int amount, float markUp)
    {
        var baseValue = data.GoldValue * amount;
        return Mathf.RoundToInt(baseValue + baseValue * markUp);
    }

    private void UpdateItemPreview(ShopSlotUI shopSlotUI)
    {
        
    }

    
}
