using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int targetScore = 100;
    
    public UnityAction OnLevelCompleted;
    
    private void Update()
    {
        CheckLevelCompletion();
    }
    
    private void CheckLevelCompletion()
    {
        if (ScoreManager.Instance.CurrentScore >= targetScore)
        {
            Debug.Log("Seviye tamamlandı!");
            OnLevelCompleted?.Invoke();
        }
    }
}
