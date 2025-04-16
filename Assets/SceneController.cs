using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Singleton pattern
    public static SceneController Instance { get; private set; }

    [Header("Scene Transition Settings")]
    [SerializeField] private float transitionDelay = 1.0f;
    [SerializeField] private GameObject loadingScreen;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Initialize loading screen if exists
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }

    // Scene değiştirme fonksiyonu
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    // Asenkron sahne yükleme
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Show loading screen if available
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
        
        // Transition delay for smooth experience
        yield return new WaitForSeconds(transitionDelay);
        
        // Check if scene exists in build settings
        if (SceneUtility.GetBuildIndexByScenePath(sceneName) >= 0)
        {
            // Async load operation
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            // Wait until scene is fully loaded
            while (!asyncLoad.isDone)
            {
                // You can add progress bar updates here if needed
                yield return null;
            }
        }
        else
        {
            Debug.LogError($"Scene {sceneName} does not exist in the build settings!");
        }
        
        // Hide loading screen if the scene change fails
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }
    
    // Direct scene loading if needed
    public void LoadSceneImmediate(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    // Restart current scene
    public void RestartCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadScene(currentSceneName);
    }
    
    // Quit application
    public void QuitGame()
    {
        Debug.Log("Exiting game...");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}