using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    public int CurrentScore { get; private set; }
    
    public UnityAction<int> OnScoreChanged;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void AddPoints(int points)
    {
        CurrentScore += points;
        OnScoreChanged?.Invoke(CurrentScore);
        Debug.Log($"Skor güncellendi: {CurrentScore}");
    }
    
    public void SubtractPoints(int points)
    {
        CurrentScore -= points;
        OnScoreChanged?.Invoke(CurrentScore);
        Debug.Log($"Skor güncellendi: {CurrentScore}");
    }
    
    public void ResetScore()
    {
        CurrentScore = 0;
        OnScoreChanged?.Invoke(CurrentScore);
    }
}