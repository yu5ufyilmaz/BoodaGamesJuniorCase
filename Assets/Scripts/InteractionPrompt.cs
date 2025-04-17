using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionPrompt : MonoBehaviour
{
    [SerializeField] private Image promptIcon;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private GameObject containerObject;
    
    private void Awake()
    {
        // Hide the prompt by default
        HidePrompt();
    }
    
    public void ShowPrompt()
    {
        if (containerObject != null)
            containerObject.SetActive(true);
    }
    
    public void HidePrompt()
    {
        if (containerObject != null)
            containerObject.SetActive(false);
    }
    
    // Optionally allow changing the prompt text for different interactions
    public void SetPromptText(string text)
    {
        if (promptText != null)
            promptText.text = text;
    }
}