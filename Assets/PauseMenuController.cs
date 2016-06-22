using UnityEngine;
using System.Collections;
using UnityEngine.UI;// we need this namespace in order to access UI elements within our script
using UnityEngine.SceneManagement; // neded in order to load scenes
using System.IO;


public class PauseMenuController : MonoBehaviour
{

    public Button Resume;
    public Button MainMenu;
    public Button Exit;
    public CanvasGroup canvasGroup;


    // Use this for initialization
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame() //This function will be used on our "Yes" button in our Quit menu
    {
        Application.Quit(); //this will quit our game. Note this will only work after building the game

    }

    public void ResumeFromPause()
    {
        Time.timeScale = 1;
        if (canvasGroup != null)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }
}