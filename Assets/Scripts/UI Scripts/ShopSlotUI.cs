using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopSlotUI : MonoBehaviour
{

    [SerializeField] private Image _itemSprite;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemCount;
    [SerializeField] private ShopSlot _assignedItemSlot;

    [SerializeField] private Button _addItemToCartButton;
    [SerializeField] private Button _removeItemToCartButton;

    public ShopKeeperDisplay ParentDisplay { get; private set; }
    public float MarkUp {  get; private set; }

    private void Awake()
    {
        _itemSprite.sprite = null;
        _itemSprite.preserveAspect = true;
        _itemSprite.color = Color.clear;
        _itemName.text = "";
        _itemCount.text = "";

        _addItemToCartButton?.onClick.AddListener(AddItemToCart);
        _removeItemToCartButton?.onClick.AddListener(RemoveItemFromCart);
        ParentDisplay = transform.parent.GetComponentInParent<ShopKeeperDisplay>();
    }

    private void UpdateUISlot()
    {
        if (_assignedItemSlot.ItemData != null)
        {
            _itemSprite.sprite = _assignedItemSlot.ItemData.Icon;
            _itemSprite.color = Color.white;
            _itemCount.text = _assignedItemSlot.StackSize.ToString();
            _itemName.text += $"{_assignedItemSlot.ItemData.DisplayName} - {_assignedItemSlot.ItemData.GoldValue}G";
        }
        else
        {
            _itemSprite.sprite = null;
            _itemSprite.color = Color.clear;
            _itemName.text = "";
            _itemCount.text = "";
        }
    }

    private void RemoveItemFromCart()
    {
        Debug.Log("removing");

    }

    private void AddItemToCart()
    {
        Debug.Log("adding");
    }

    public void Init(ShopSlot slot,float markUp)
    {
        _assignedItemSlot = slot;   
        UpdateUISlot();
        MarkUp = markUp;
    }
}
