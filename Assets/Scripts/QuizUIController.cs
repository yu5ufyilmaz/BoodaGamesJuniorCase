using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizUIController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject welcomePanel;
    [SerializeField] private GameObject quizPanel;
    
    [Header("Welcome Screen")]
    [SerializeField] private TextMeshProUGUI welcomeScoreText;
    [SerializeField] private Button startQuizButton;
    
    [Header("Quiz Navigation")]
    [SerializeField] private Button quitButton;
    
    private void Awake()
    {
        // Initial UI setup
        welcomePanel.SetActive(true);
        quizPanel.SetActive(false);
        
        // Get initial score
        int initialScore = 0;
        if (PlayerPrefs.HasKey("GameScore"))
        {
            initialScore = PlayerPrefs.GetInt("GameScore");
        }
        
        // Set welcome text
        welcomeScoreText.text = $"Welcome to the Quiz!\nYour current score: {initialScore}";
        
        // Set up button listeners
        startQuizButton.onClick.AddListener(StartQuiz);
        quitButton.onClick.AddListener(QuitToMainMenu);
    }
    
    public void StartQuiz()
    {
        welcomePanel.SetActive(false);
        quizPanel.SetActive(true);
    }
    
    public void QuitToMainMenu()
    {
        PlayerPrefs.SetInt("GameScore", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("StartScene");
    }
    
    // Animation effects for correct/incorrect answers
    public void PlayCorrectAnswerAnimation(TextMeshProUGUI feedbackText)
    {
        StartCoroutine(PulseText(feedbackText, Color.green));
    }
    
    public void PlayIncorrectAnswerAnimation(TextMeshProUGUI feedbackText)
    {
        StartCoroutine(PulseText(feedbackText, Color.red));
    }
    
    private IEnumerator PulseText(TextMeshProUGUI text, Color color)
    {
        // Save original values
        Color originalColor = text.color;
        float originalScale = text.transform.localScale.x;
        
        // Animation
        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            float scale = Mathf.Lerp(originalScale, originalScale * 1.5f, t / 0.5f);
            text.transform.localScale = new Vector3(scale, scale, scale);
            text.color = color;
            yield return null;
        }
        
        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            float scale = Mathf.Lerp(originalScale * 1.5f, originalScale, t / 0.5f);
            text.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        
        // Reset
        text.transform.localScale = new Vector3(originalScale, originalScale, originalScale);
        text.color = originalColor;
    }
}