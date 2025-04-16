using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    public int CurrentScore { get; private set; }
    
    // Skor değiştiğinde triggerlanan event
    public UnityAction<int> OnScoreChanged;
    
    // PlayerPrefs için anahtar
    private const string SCORE_KEY = "GameScore";
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahneler arası korunacak
            
            // Eğer PlayerPrefs'te kaydedilmiş bir skor varsa, onu yükle
            LoadScore();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Debug kontrolü
        Debug.Log($"ScoreManager başlatıldı. Mevcut skor: {CurrentScore}");
    }
    
    // PlayerPrefs'ten skoru yükle
    private void LoadScore()
    {
        if (PlayerPrefs.HasKey(SCORE_KEY))
        {
            CurrentScore = PlayerPrefs.GetInt(SCORE_KEY);
            Debug.Log($"Loaded score from PlayerPrefs: {CurrentScore}");
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
    
    // Skoru kaydet
    private void SaveScore()
    {
        PlayerPrefs.SetInt(SCORE_KEY, CurrentScore);
        PlayerPrefs.Save();
        Debug.Log($"Saved score to PlayerPrefs: {CurrentScore}");
    }
    
    public void AddPoints(int points)
    {
        CurrentScore += points;
        OnScoreChanged?.Invoke(CurrentScore);
        Debug.Log($"Skor güncellendi: {CurrentScore} (+{points})");
        SaveScore();
    }
    
    public void SubtractPoints(int points)
    {
        CurrentScore -= points;
        OnScoreChanged?.Invoke(CurrentScore);
        Debug.Log($"Skor güncellendi: {CurrentScore} (-{points})");
        SaveScore();
    }
    
    public void ResetScore()
    {
        CurrentScore = 0;
        OnScoreChanged?.Invoke(CurrentScore);
        SaveScore();
        Debug.Log("Skor sıfırlandı");
    }
}