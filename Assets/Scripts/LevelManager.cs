using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    
    public void LoadLevel(string levelName)
    {
        PlayerPrefs.SetInt("GameScore", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene(levelName);
    }
    
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting...");
    }
}
