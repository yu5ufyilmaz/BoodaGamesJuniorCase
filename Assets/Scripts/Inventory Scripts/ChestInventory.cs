using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChestInventory : InventoryHolder, IInteractable
{
    public UnityAction<IInteractable> OnInteractionComplete { get; set; }

    public void Interact(Interactor interactor, out bool interactSuccesful)
    {
        OnDynamicInventoryDisplayRequested?.Invoke((primaryInventorySystem));
        interactSuccesful = true;
    }

    public void EndInteraction()
    {
        throw new System.NotImplementedException();
    }
    
}
