using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[System.Serializable]
public class InventoryHolder : MonoBehaviour
{
  [SerializeField] private int inventorySize;
  [FormerlySerializedAs("inventorySystem")] [SerializeField] protected InventorySystem primaryInventorySystem;

  public InventorySystem PrimaryInventorySystem => primaryInventorySystem;
  public static UnityAction<InventorySystem> OnDynamicInventoryDisplayRequested;

  protected virtual void Awake()
  {
    primaryInventorySystem = new InventorySystem(inventorySize);
  }
}
