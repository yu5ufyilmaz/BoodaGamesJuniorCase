using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InteractionPromptUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject promptContainer;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Image keyImage;
    [SerializeField] private TextMeshProUGUI keyText;
    
    [Header("Settings")]
    [SerializeField] private string defaultPromptText = "Interact";
    [SerializeField] private string defaultKeyText = "E";
    [SerializeField] private float fadeInDuration = 0.25f;
    [SerializeField] private float fadeOutDuration = 0.2f;
    
    // Animation variables
    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;
    
    private void Awake()
    {
        // Get or add CanvasGroup component
        canvasGroup = promptContainer.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = promptContainer.AddComponent<CanvasGroup>();
        
        // Hide prompt initially
        canvasGroup.alpha = 0f;
        promptContainer.SetActive(false);
    }
    
    private void Start()
    {
        // Subscribe to interaction events from the Interactor
        Interactor interactor = FindObjectOfType<Interactor>();
        if (interactor != null)
        {
            interactor.OnInteractableFound += ShowPrompt;
            interactor.OnInteractableLost += HidePrompt;
        }
        else
        {
            Debug.LogWarning("InteractionPromptUI: No Interactor found in the scene!");
        }
    }
    
    public void ShowPrompt(string customText = "")
    {
        // Set text
        promptText.text = string.IsNullOrEmpty(customText) ? defaultPromptText : customText;
        keyText.text = defaultKeyText;
        
        // Show and fade in
        promptContainer.SetActive(true);
        
        // Stop any current fade
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        
        // Start new fade in
        fadeCoroutine = StartCoroutine(FadeIn());
    }
    
    public void HidePrompt()
    {
        // Stop any current fade
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        
        // Start fade out
        fadeCoroutine = StartCoroutine(FadeOut());
    }
    
    private IEnumerator FadeIn()
    {
        float time = 0;
        float startAlpha = canvasGroup.alpha;
        
        while (time < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, time / fadeInDuration);
            time += Time.deltaTime;
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }
    
    private IEnumerator FadeOut()
    {
        float time = 0;
        float startAlpha = canvasGroup.alpha;
        
        while (time < fadeOutDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / fadeOutDuration);
            time += Time.deltaTime;
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        promptContainer.SetActive(false);
    }
}