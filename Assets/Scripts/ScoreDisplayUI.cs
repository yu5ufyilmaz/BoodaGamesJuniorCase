using UnityEngine;
using TMPro;

public class ScoreDisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    
    private void OnEnable()
    {
        ScoreManager.Instance.OnScoreChanged += UpdateScoreText;
        UpdateScoreText(ScoreManager.Instance.CurrentScore);
    }
    
    private void OnDisable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreText;
    }
    
    private void UpdateScoreText(int newScore)
    {
        scoreText.text = $"Puan: {newScore}";
    }
}