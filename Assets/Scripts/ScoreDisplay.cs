using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private string scorePrefix = "Score: ";
    [SerializeField] private bool showAnimatedScore = false;
    [SerializeField] private float scoreAnimationDuration = 1.0f;
    
    private int currentDisplayScore = 0;
    private int targetScore = 0;
    private float animationTimer = 0f;
    
    private void Start()
    {
        if (scoreText == null)
        {
            scoreText = GetComponent<TextMeshProUGUI>();
            if (scoreText == null)
            {
                Debug.LogError("ScoreDisplay requires a TextMeshProUGUI component!");
                enabled = false;
                return;
            }
        }
        
        // PlayerPrefs'ten skoru yükle
        LoadScoreFromPlayerPrefs();
    }
    
    private void OnEnable()
    {
        // Eğer ScoreManager varsa, skor değişim olaylarını dinle
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScoreText;
            UpdateScoreText(ScoreManager.Instance.CurrentScore);
        }
        else
        {
            // ScoreManager yoksa PlayerPrefs'ten skoru yükle
            LoadScoreFromPlayerPrefs();
        }
    }
    
    private void OnDisable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreText;
        }
    }
    
    private void LoadScoreFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("GameScore"))
        {
            int score = PlayerPrefs.GetInt("GameScore");
            Debug.Log($"Loaded score from PlayerPrefs: {score}");
            
            if (showAnimatedScore)
            {
                StartScoreAnimation(score);
            }
            else
            {
                UpdateScoreText(score);
            }
        }
        else
        {
            UpdateScoreText(0);
            Debug.Log("No saved score found in PlayerPrefs");
        }
    }
    
    private void UpdateScoreText(int newScore)
    {
        if (scoreText != null)
        {
            if (showAnimatedScore)
            {
                StartScoreAnimation(newScore);
            }
            else
            {
                scoreText.text = $"{scorePrefix}{newScore}";
                currentDisplayScore = newScore;
            }
        }
    }
    
    private void StartScoreAnimation(int newScore)
    {
        targetScore = newScore;
        animationTimer = 0f;
    }
    
    private void Update()
    {
        if (showAnimatedScore && currentDisplayScore != targetScore)
        {
            animationTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(animationTimer / scoreAnimationDuration);
            
            // Lerp, skor animasyonu için
            currentDisplayScore = Mathf.RoundToInt(Mathf.Lerp(currentDisplayScore, targetScore, progress));
            scoreText.text = $"{scorePrefix}{currentDisplayScore}";
            
            if (progress >= 1.0f)
            {
                currentDisplayScore = targetScore;
                scoreText.text = $"{scorePrefix}{currentDisplayScore}";
            }
        }
    }
}