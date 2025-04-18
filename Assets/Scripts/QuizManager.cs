using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class QuizQuestion
    {
        public string questionText;
        public string[] answerOptions = new string[4];
        public int correctAnswerIndex;
        public int pointsForCorrect = 10;
        public int pointsForIncorrect = -5;
    }

    [Header("Quiz Settings")]
    [SerializeField] private List<QuizQuestion> questions = new List<QuizQuestion>();
    [SerializeField] private int currentQuestionIndex = 0;
    [SerializeField] private float delayBetweenQuestions = 1.5f;
    [SerializeField] private float totalQuizTime = 120f; // Quiz için toplam süre (saniye)
    [SerializeField] private TextMeshProUGUI timerText; // Timer metni
    [SerializeField] private Slider timerSlider; // Timer için slider
    [SerializeField] private Color normalTimerColor = Color.green; // Normal renk
    [SerializeField] private Color warningTimerColor = Color.yellow; // Uyarı rengi
    [SerializeField] private Color criticalTimerColor = Color.red; // Kritik renk
    [SerializeField] private float warningThreshold = 0.5f; // Uyarı eşiği
    [SerializeField] private float criticalThreshold = 0.25f; // Kritik eşik
    
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TextMeshProUGUI[] answerTexts;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI quizScoreText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    
    [Header("Button Colors")]
    [SerializeField] private Color defaultButtonColor = Color.white;
    [SerializeField] private Color correctButtonColor = Color.green;
    [SerializeField] private Color incorrectButtonColor = Color.red;
    
    private int initialScore = 0;
    private int currentScore = 0;
    private int quizScore = 0;
    private bool canAnswer = true;
    private Image[] buttonImages;
    
    // Timer değişkenleri
    private float remainingTime;
    private bool isTimerRunning = false;
    private Image timerFillImage; // Slider'ın fill image'ı
    
    private void Awake()
    {
        // Initialize button images array
        buttonImages = new Image[answerButtons.Length];
        for (int i = 0; i < answerButtons.Length; i++)
        {
            buttonImages[i] = answerButtons[i].GetComponent<Image>();
        }
        
        // Load score from previous scene
        if (PlayerPrefs.HasKey("GameScore"))
        {
            initialScore = PlayerPrefs.GetInt("GameScore");
            currentScore = initialScore;
            Debug.Log($"Loaded initial score: {initialScore}");
        }
        else
        {
            Debug.LogWarning("No score found in PlayerPrefs. Starting with 0.");
        }
        
        // Hide results panel initially
        resultPanel.SetActive(false);
        questionPanel.SetActive(true);
        
        // Update score display
        UpdateScoreText();
        
        // Timer'ı başlat
        if (timerSlider != null)
        {
            timerFillImage = timerSlider.fillRect.GetComponent<Image>();
        }
        
        // Timer'ı başlat
        remainingTime = totalQuizTime;
        isTimerRunning = true;
        UpdateTimerDisplay();
    }
    
    private void Start()
    {
        // Load the first question
        LoadQuestion(currentQuestionIndex);
    }
    
    private void Update()
    {
        if (isTimerRunning)
        {
            // Kalan süreyi güncelle
            remainingTime -= Time.deltaTime;
            
            // Timer göstergesini güncelle
            UpdateTimerDisplay();
            
            // Süre doldu mu kontrol et
            if (remainingTime <= 0)
            {
                remainingTime = 0;
                isTimerRunning = false;
                OnTimerExpired();
            }
        }
    }
    
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            // Süreyi dakika:saniye formatında göster
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            
            // Sürenin azalmasına göre renk değiştir
            float timeRatio = remainingTime / totalQuizTime;
            
            if (timeRatio <= criticalThreshold)
            {
                timerText.color = criticalTimerColor;
            }
            else if (timeRatio <= warningThreshold)
            {
                timerText.color = warningTimerColor;
            }
            else
            {
                timerText.color = normalTimerColor;
            }
        }
        
        // Opsiyonel: Fill image ile görsel geri bildirim
        if (timerSlider != null)
        {
            timerSlider.value = remainingTime / totalQuizTime;
            
            // Fill rengini güncelle
            if (timerFillImage != null)
            {
                if (remainingTime / totalQuizTime <= criticalThreshold)
                {
                    timerFillImage.color = criticalTimerColor;
                }
                else if (remainingTime / totalQuizTime <= warningThreshold)
                {
                    timerFillImage.color = warningTimerColor;
                }
                else
                {
                    timerFillImage.color = normalTimerColor;
                }
            }
        }
    }
    
    private void OnTimerExpired()
    {
        Debug.Log("Süre doldu! Quiz sonlandırılıyor.");
        
        // Kullanıcının cevap vermesini engelle
        canAnswer = false;
        
        // Tüm aktif coroutine'leri durdur
        StopAllCoroutines();
        
        // Sonuç ekranını göster
        ShowResults();
    }
    
    private void LoadQuestion(int index)
    {
        if (index < 0 || index >= questions.Count)
        {
            Debug.LogError($"Question index {index} out of range!");
            return;
        }
        
        // Reset button colors
        ResetButtonColors();
        
        // Set question text
        questionText.text = questions[index].questionText;
        
        // Set answer texts
        for (int i = 0; i < answerTexts.Length; i++)
        {
            if (i < questions[index].answerOptions.Length)
            {
                answerTexts[i].text = questions[index].answerOptions[i];
                answerButtons[i].gameObject.SetActive(true);
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
        
        // Clear feedback text
        feedbackText.text = "";
        
        // Enable answering
        canAnswer = true;
    }
    
    public void AnswerSelected(int selectedIndex)
    {
        if (!canAnswer) return;
        
        canAnswer = false;
        QuizQuestion currentQuestion = questions[currentQuestionIndex];
        
        // Check if answer is correct
        bool isCorrect = selectedIndex == currentQuestion.correctAnswerIndex;
        
        // Set button colors
        if (isCorrect)
        {
            buttonImages[selectedIndex].color = correctButtonColor;
            feedbackText.text = "Correct!";
            feedbackText.color = correctButtonColor;
            quizScore += currentQuestion.pointsForCorrect;
            currentScore += currentQuestion.pointsForCorrect;
        }
        else
        {
            buttonImages[selectedIndex].color = incorrectButtonColor;
            buttonImages[currentQuestion.correctAnswerIndex].color = correctButtonColor;
            feedbackText.text = "Incorrect!";
            feedbackText.color = incorrectButtonColor;
            quizScore += currentQuestion.pointsForIncorrect;
            currentScore += currentQuestion.pointsForIncorrect;
        }
        
        // Update score
        UpdateScoreText();
        
        // Move to next question after delay
        StartCoroutine(NextQuestionAfterDelay());
    }
    
    private IEnumerator NextQuestionAfterDelay()
    {
        yield return new WaitForSeconds(delayBetweenQuestions);
        
        currentQuestionIndex++;
        
        if (currentQuestionIndex < questions.Count)
        {
            LoadQuestion(currentQuestionIndex);
        }
        else
        {
            // Tüm sorular bitti, zamanı durdur
            isTimerRunning = false;
            ShowResults();
        }
    }
    
    private void ShowResults()
    {
        // Hide question panel and show results panel
        questionPanel.SetActive(false);
        resultPanel.SetActive(true);
        
        // Update final score texts
        finalScoreText.text = $"Final Score: {currentScore}";
        quizScoreText.text = $"Quiz Points: {quizScore}";
        totalScoreText.text = $"Initial Score: {initialScore}";
        
        // Sürenin dolup dolmadığını göster
        if (remainingTime <= 0)
        {
            // Opsiyonel: Süre dolduğuna dair ekstra geri bildirim ekleyebilirsiniz
            feedbackText.text = "Time's up!";
            feedbackText.color = criticalTimerColor;
        }
        
        // Save the final score
        PlayerPrefs.SetInt("GameScore", currentScore);
        PlayerPrefs.Save();
    }
    
    private void UpdateScoreText()
    {
        scoreText.text = $"Score: {currentScore}";
    }
    
    private void ResetButtonColors()
    {
        for (int i = 0; i < buttonImages.Length; i++)
        {
            buttonImages[i].color = defaultButtonColor;
        }
    }
    
    public void RestartQuiz()
    {
        PlayerPrefs.SetInt("GameScore", 0);
        PlayerPrefs.Save();
        // Reset quiz state
        currentQuestionIndex = 0;
        quizScore = 0;
        currentScore = initialScore;
        
        // Reset timer
        remainingTime = totalQuizTime;
        isTimerRunning = true;
        
        // Update UI
        UpdateScoreText();
        UpdateTimerDisplay();
        questionPanel.SetActive(true);
        resultPanel.SetActive(false);
        
        // Load first question
        LoadQuestion(currentQuestionIndex);
    }
    
    public void GoToMainScene()
    {
        PlayerPrefs.SetInt("GameScore", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Menu");
    }
    
    // Zamanlayıcı ile ilgili yardımcı metodlar
    public void PauseTimer()
    {
        isTimerRunning = false;
    }
    
    public void ResumeTimer()
    {
        isTimerRunning = true;
    }
    
    public void AddTime(float secondsToAdd)
    {
        remainingTime += secondsToAdd;
        UpdateTimerDisplay();
    }
}