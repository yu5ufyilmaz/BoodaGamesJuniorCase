using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 10f;     
    [SerializeField] private int targetScore = 100; 
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI resultText;
    
    private float timeRemaining;
    private int currentScore = 0;
    private bool isGameActive = false;
    
    
    public static UnityAction OnGameOver;
    
    private PlayerInventoryHolder playerInventory;
    
    private void Awake()
    {
        // Find the player inventory in the scene
        playerInventory = FindObjectOfType<PlayerInventoryHolder>();
        
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventoryHolder not found in the scene!");
        }
        
        // Initialize game state
        timeRemaining = gameDuration;
        gameOverPanel.SetActive(false);
        UpdateScoreDisplay();
    }
    
    private void Start()
    {
        // Start the game
        StartGame();
    }
    
    private void Update()
    {
        if (isGameActive)
        {
            // Update timer
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                // Time's up
                EndGame();
            }
        }
    }
    
    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        if (timeRemaining < 30)
        {
            timerText.color = Color.red;
        }
    }
    
    private void UpdateScoreDisplay()
    {
        scoreText.text = "Score: " + currentScore;
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();
    }
    
    public void StartGame()
    {
        isGameActive = true;
        timeRemaining = gameDuration;
        currentScore = 0;
        UpdateScoreDisplay();
        UpdateTimerDisplay();
    }
    
    public void EndGame()
    {
        isGameActive = false;
        
        // Calculate final score based on items in inventory
        int finalScore = CalculateFinalScore();
        
        // Show game over panel
        gameOverPanel.SetActive(true);
        finalScoreText.text = "Final Score: " + finalScore;
        
        // Determine if player won or lost
        if (finalScore >= targetScore)
        {
            resultText.text = "You Win!";
            resultText.color = Color.green;
        }
        else
        {
            resultText.text = "Try Again!";
            resultText.color = Color.red;
        }
        
        // Trigger game over event
        OnGameOver?.Invoke();
    }
    
    private int CalculateFinalScore()
    {
        int score = 0;
        
        // Get all items in player's inventory
        var itemsHeld = playerInventory.PrimaryInventorySystem.GetAllItemsHeld();
        
        // Calculate score based on items
        foreach (var itemKvp in itemsHeld)
        {
            InventoryItemData item = itemKvp.Key;
            int amount = itemKvp.Value;
            
            if (item.IsCorrectItem)
            {
                // Add points for correct items based on the PointValue property
                score += item.PointValue * amount;
            }
            else
            {
                // Subtract points for incorrect items
                // Using the actual PointValue property for incorrect items too
                score -= item.PointValue * amount; 
            }
        }
        
        // Ensure score doesn't go below zero
        return Mathf.Max(0, score);
    }
    
    public void RestartGame()
    {
        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    
    public void QuitGame()
    {
        // Quit the application (works in build only)
        Application.Quit();
        
        // For testing in the editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}