using UnityEngine;
using UnityEngine.UI;// we need this namespace in order to access UI elements within our script
using System.Collections;
using UnityEngine.SceneManagement; // neded in order to load scenes
using System.IO;

public class MenuController : MonoBehaviour
{
    public Canvas quitMenu;
    public Button startText;
    public Button exitText;
    public Dropdown dropdown;

    //this is so fucking confusing
    //i think i need to add onclick events to fire when buttons are pressed
    // todo figure out how that works
    
    void Start()
    {
        dropdown.ClearOptions();
        string[] SceneNames = new string[] { "Level_test_1", "Test_Game", "Door Test", "Obstacle_Course"};

        foreach (string name in SceneNames)
        {
            Dropdown.OptionData list = new Dropdown.OptionData(name);
            dropdown.options.Add(list);
        }
        dropdown.RefreshShownValue();


        quitMenu = quitMenu.GetComponent<Canvas>();
        startText = startText.GetComponent<Button>();
        exitText = exitText.GetComponent<Button>();
        quitMenu.enabled = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }
    
    public void ExitPress() //this function will be used on our Exit button

    {
        quitMenu.enabled = true; //enable the Quit menu when we click the Exit button
        startText.enabled = false; //then disable the Play and Exit buttons so they cannot be clicked
        exitText.enabled = false;

    }

    public void NoPress() //this function will be used for our "NO" button in our Quit Menu

    {
        quitMenu.enabled = false; //we'll disable the quit menu, meaning it won't be visible anymore
        startText.enabled = true; //enable the Play and Exit buttons again so they can be clicked
        exitText.enabled = true;

    }

    public void StartLevel() //this function will be used on our Play button
    {
        //Debug.Log("play button pressed");
        //Debug.Log(dropdown.captionText.text);
        SceneManager.LoadScene(dropdown.captionText.text); //this will load our first level from our build settings. "1" is the second scene in our game

    }

    public void ExitGame() //This function will be used on our "Yes" button in our Quit menu

    {
        Application.Quit(); //this will quit our game. Note this will only work after building the game

    }

    public void DeselectElement()
    {
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }

}