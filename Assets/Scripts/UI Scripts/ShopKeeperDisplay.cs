using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopKeeperDisplay : MonoBehaviour
{
   [SerializeField] private ShopSlotUI _shopSlotPrefab;
   [SerializeField] private ShoppingCartItemUI _shoppingCartItemPrefab;

   [SerializeField] private Button _buyTab;
   [SerializeField] private Button _sellTab;

   [Header("Shopping Cart")] 
   [SerializeField] private TextMeshProUGUI _basketTotalText;
   [SerializeField] private TextMeshProUGUI _playerGoldText;
   [SerializeField] private TextMeshProUGUI _shopGoldText;
   [SerializeField] private Button _buyButton;
   [SerializeField] private TextMeshProUGUI _buyButtonText;
   
   // Sonraki sahneye geçiş için
   [Header("Navigation")]
   [SerializeField] private string nextSceneName = "NextScene";

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

        // Shop'ta satma özelliğini devre dışı bırakalım, sadece satın alma olsun
        if (_sellTab != null)
            _sellTab.gameObject.SetActive(false);
            
        if (_buyTab != null)
            _buyTab.gameObject.SetActive(false);
            
        _isSelling = false;

        // Sadece BuySell butonu aktif olacak
        _buyButton.gameObject.SetActive(true);
        _buyButtonText.text = "Buy Items";
        
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        if (_buyButton != null)
        {
            _buyButtonText.text = "Buy Items";
            _buyButton.onClick.RemoveAllListeners();
            _buyButton.onClick.AddListener(BuyItems);
            
            // Buton her zaman aktif kalacak
            _buyButton.gameObject.SetActive(true);
        }
        
        ClearSlots();
        ClearItemPreview();

        _basketTotalText.enabled = false;
        _basketTotal = 0;
        _playerGoldText.text = $"Player Gold: {_playerInventoryHolder.PrimaryInventorySystem.Gold}";
        _shopGoldText.text = $"Shop Gold: {_shopSystem.AvailableGold}";

        // Shop envanteri yerine, oyuncunun topladığı öğeleri gösteriyoruz
        DisplayPlayerCollectedItems();
    }

    private void BuyItems()
    {
        if (_playerInventoryHolder.PrimaryInventorySystem.Gold < _basketTotal)
        {
            Debug.Log("Not enough gold to complete purchase!");
            return;
        }

        int totalScore = 0;

        // Satın alma işlemini gerçekleştir ve skorları hesapla
        foreach (var kvp in _shoppingCart)
        {
            _shopSystem.PurchaseItem(kvp.Key, kvp.Value);
            
            // Correct item'a göre skor hesaplaması
            if (kvp.Key.IsCorrectItem)
            {
                int itemScore = kvp.Key.PointValue * kvp.Value;
                totalScore += itemScore;
                Debug.Log($"Gained {itemScore} points from {kvp.Value}x {kvp.Key.DisplayName}");
            }
        }

        // Skoru ScoreManager'a kaydet
        ScoreManager.Instance.AddPoints(totalScore);
        Debug.Log($"Total score: {ScoreManager.Instance.CurrentScore}");

        // Para transferi
        _shopSystem.GainGold(_basketTotal);
        _playerInventoryHolder.PrimaryInventorySystem.SpendGold(_basketTotal);
        
        // Satın alma işlemi tamamlandıktan sonra, sonraki sahneye geç
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        // Show a loading message or transition effect
        if (_basketTotalText != null)
        {
            _basketTotalText.text = "Preparing quiz...";
        }
    
        // Geçiş efekti veya bekleme süresi
        yield return new WaitForSeconds(1.0f);
    
        // Skoru PlayerPrefs'e kaydedelim böylece sonraki sahnede erişebiliriz
        PlayerPrefs.SetInt("GameScore", ScoreManager.Instance.CurrentScore);
        PlayerPrefs.Save();
    
        Debug.Log($"Saving score {ScoreManager.Instance.CurrentScore} to PlayerPrefs");
    
        // Make sure TestScene is added to your Build Settings!
        SceneManager.LoadScene("TestScene");
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
    
    // Oyuncunun topladığı öğeleri göster
    private void DisplayPlayerCollectedItems()
    {
        // Oyuncunun topladığı öğeleri göster
        foreach (var item in _playerInventoryHolder.PrimaryInventorySystem.GetAllItemsHeld())
        {
            var tempSlot = new ShopSlot();
            tempSlot.AssignItem(item.Key, item.Value);

            var shopSlot = Instantiate(_shopSlotPrefab, _itemListContentPanel.transform);
            shopSlot.Init(tempSlot, _shopSystem.BuyMarkUp);
        }
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
        _itemPreviewSprite.sprite = null;
        _itemPreviewSprite.color = Color.clear;
        _itemPreviewName.text = "";
        _itemPreviewDescription.text = "";
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
        var goldToCheck = _playerInventoryHolder.PrimaryInventorySystem.Gold;
        _basketTotalText.color = _basketTotal > goldToCheck ? Color.red : Color.green;
    }

    public static int GetModifiedPrice(InventoryItemData data, int amount, float markUp)
    {
        var baseValue = data.GoldValue * amount;
        return Mathf.FloorToInt(baseValue + baseValue * markUp);
    }

    private void UpdateItemPreview(ShopSlotUI shopSlotUI)
    {
        var data = shopSlotUI.AssignedItemSlot.ItemData;
        _itemPreviewSprite.sprite = data.Icon;
        _itemPreviewSprite.color = Color.white;
        _itemPreviewName.text = data.DisplayName;
        _itemPreviewDescription.text = data.Description;
    }

    public void OnBuyTabPressed()
    {
        _isSelling = false;
        RefreshDisplay();
    }
}