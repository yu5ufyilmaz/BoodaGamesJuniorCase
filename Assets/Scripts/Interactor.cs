using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Interactor : MonoBehaviour
{
    public Transform InteractionPoint; // Kameranın transform'u
    public LayerMask InteractionLayer;
    public float InteractionDistance = 3f; // Etkileşim mesafesi
    
    public bool IsInteracting { get; private set; }
    
    // Events for UI prompt
    public Action<string> OnInteractableFound;
    public Action OnInteractableLost;
    
    private Camera mainCamera;
    private IInteractable currentInteractable;
    private bool isLookingAtInteractable = false;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Ekranın ortasından bir ışın gönder
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        // Debug için ışını görselleştir
        Debug.DrawRay(ray.origin, ray.direction * InteractionDistance, Color.red);
        
        bool hitInteractable = false;
        
        // Işının çarptığı nesne varsa
        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractionDistance, InteractionLayer))
        {
            // Çarpılan nesnenin IInteractable arayüzünü uygulayıp uygulamadığını kontrol et
            var interactable = hitInfo.collider.GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                hitInteractable = true;
                currentInteractable = interactable;
                
                // If we just started looking at an interactable
                if (!isLookingAtInteractable)
                {
                    isLookingAtInteractable = true;
                    
                    // Get the name of the object if possible for a custom message
                    string objectName = hitInfo.collider.gameObject.name;
                    string displayName = "";
                    
                    // Try to get a better name from the object
                    var itemPickup = hitInfo.collider.GetComponent<ItemPickUp>();
                    if (itemPickup != null && itemPickup.ItemData != null)
                    {
                        displayName = $"Pick up {itemPickup.ItemData.DisplayName}";
                    }
                    else
                    {
                        // Generic interaction prompt
                        displayName = "Interact";
                    }
                    
                    // Trigger UI prompt
                    OnInteractableFound?.Invoke(displayName);
                }
                
                // E tuşuna basılmışsa etkileşime geç
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    StartInteraction(interactable);
                }
            }
        }
        
        // If we were looking at an interactable but no longer are
        if (isLookingAtInteractable && !hitInteractable)
        {
            isLookingAtInteractable = false;
            currentInteractable = null;
            OnInteractableLost?.Invoke();
        }
    }
    
    void StartInteraction(IInteractable interactable)
    {
        interactable.Interact(this, out bool interactSuccessful);
        IsInteracting = interactSuccessful;
        
        // If interaction was successful, hide the prompt temporarily
        if (interactSuccessful)
        {
            OnInteractableLost?.Invoke();
        }
    }

    void EndInteraction()
    {
        IsInteracting = false;
    }
}