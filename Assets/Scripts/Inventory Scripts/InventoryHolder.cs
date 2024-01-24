using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[System.Serializable]
public abstract class InventoryHolder : MonoBehaviour
{
  [SerializeField] private int inventorySize;
  [FormerlySerializedAs("inventorySystem")] [SerializeField] protected InventorySystem primaryInventorySystem;
  [SerializeField] protected int offset = 10;
  [SerializeField] protected int _gold;


  public int Offset => offset;

  public InventorySystem PrimaryInventorySystem => primaryInventorySystem;
  public static UnityAction<InventorySystem,int> OnDynamicInventoryDisplayRequested;

  protected virtual void Awake()
  {
    SaveLoad.OnLoadGame += LoadInventory;
    
    primaryInventorySystem = new InventorySystem(inventorySize, _gold);
  }

  protected abstract void LoadInventory(SaveData saveData);
}


[System.Serializable]
public struct InventorySaveData
{
  [FormerlySerializedAs("invSystem")] public InventorySystem InvSystem;
  public Vector3 position;
  public Quaternion rotation;

  public InventorySaveData(InventorySystem _invSystem, Vector3 _position, Quaternion _rotation)
  {
    InvSystem = _invSystem;
    position = _position;
    rotation = _rotation;
  }

  public InventorySaveData(InventorySystem _invSystem)
  {
    InvSystem = _invSystem;
    position = Vector3.zero;
    rotation = Quaternion.identity;
  }
}

