using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public Transform InteractionPoint; // Kameranın transform'u
    public LayerMask InteractionLayer;
    public float InteractionDistance = 3f; // Etkileşim mesafesi
    
    [SerializeField] private InteractionPrompt interactionPrompt; // Reference to our UI prompt
    
    public bool IsInteracting { get; private set; }
    
    private Camera mainCamera;
    private IInteractable currentInteractable;

    private void Awake()
    {
        mainCamera = Camera.main;
        
        // Make sure prompt is hidden at start
        if (interactionPrompt != null)
        {
            interactionPrompt.HidePrompt();
        }
    }

    private void Update()
    {
        // Reset current interactable
        IInteractable interactable = null;
        
        // Ekranın ortasından bir ışın gönder
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        // Debug için ışını görselleştir
        Debug.DrawRay(ray.origin, ray.direction * InteractionDistance, Color.red);
        
        // Işının çarptığı nesne varsa
        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractionDistance, InteractionLayer))
        {
            // Çarpılan nesnenin IInteractable arayüzünü uygulayıp uygulamadığını kontrol et
            interactable = hitInfo.collider.GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                // Show interaction prompt
                if (interactionPrompt != null)
                {
                    interactionPrompt.ShowPrompt();
                }
                
                // E tuşuna basılmışsa etkileşime geç
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    StartInteraction(interactable);
                }
            }
        }
        
        // If we're not looking at an interactable but were previously
        if (interactable == null && interactionPrompt != null)
        {
            interactionPrompt.HidePrompt();
        }
        
        // Keep track of current interactable
        currentInteractable = interactable;
    }
    
    void StartInteraction(IInteractable interactable)
    {
        interactable.Interact(this, out bool interactSuccessful);
        IsInteracting = interactSuccessful;
        
        // Optionally hide the prompt during interaction
        if (interactionPrompt != null && interactSuccessful)
        {
            interactionPrompt.HidePrompt();
        }
    }

    void EndInteraction()
    {
        IsInteracting = false;
    }
}