using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopKeeperDisplay : MonoBehaviour
{
   [SerializeField] private ShopSlotUI _shopSlotPrefab;

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
}
