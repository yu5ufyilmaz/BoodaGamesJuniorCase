using UnityEngine;

using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject playButon;
    public GameObject quitButton;
    public GameObject creditsButton;
    public GameObject creditsImage;
    public GameObject backButton;
    public void Play()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void Credits()
    {
        playButon.SetActive(false);
        quitButton.SetActive(false);
        creditsButton.SetActive(false);
        creditsImage.SetActive(true);
        backButton.SetActive(true);
    }
    public void Back() 
    {
        playButon.SetActive(true );
        quitButton.SetActive(true );
        creditsButton.SetActive(true );
        creditsImage.SetActive(false);
        backButton.SetActive(false);
    }
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
