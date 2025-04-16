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
    
    public bool IsInteracting { get; private set; }
    
    private Camera mainCamera;

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
        
        // Işının çarptığı nesne varsa ve E tuşuna basılmışsa
        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractionDistance, InteractionLayer))
        {
            // Çarpılan nesnenin IInteractable arayüzünü uygulayıp uygulamadığını kontrol et
            var interactable = hitInfo.collider.GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                // E tuşuna basılmışsa etkileşime geç
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    StartInteraction(interactable);
                }
            }
        }
    }
    
    void StartInteraction(IInteractable interactable)
    {
        interactable.Interact(this, out bool interactSuccessful);
        IsInteracting = interactSuccessful;
    }

    void EndInteraction()
    {
        IsInteracting = false;
    }
}