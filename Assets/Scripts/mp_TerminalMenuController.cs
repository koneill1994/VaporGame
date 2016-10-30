using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.EventSystems;

public class mp_TerminalMenuController : NetworkBehaviour
{

    public int distanceToItem;

    public Light Spotlight;
    public Slider LightSlider;

    private CanvasGroup canvasGroup;
    private bool TerminalActive = false;

    public Camera player_camera;

    public InputField t_input;
    
    //mp part is still WIP

    // Use this for initialization
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.5f; //this makes everything transparent
            canvasGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player_camera != null)
        {
            GameObject player = player_camera.gameObject.transform.parent.gameObject;

            RaycastHit hit;
            //Ray ray = GameObject.FindWithTag("client_cam").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            Ray ray = player_camera.ScreenPointToRay(Input.mousePosition);
            //see if there's a terminal in front of the player
            if (Physics.Raycast(ray, out hit, distanceToItem) && hit.collider.gameObject == gameObject)
            {
                //use button is pressed
                if (Input.GetButtonUp("Use")&& !TerminalActive)
                {
                    ToggleTerminal();
                    player.GetComponent<FirstPersonController_Centrifugal_mp>().is_paused = true;
                    t_input.Select();
                }
            }
            if (TerminalActive)
            {
                //make sure the terminal is close enough to be activated
                Vector3 offset = transform.position - player_camera.transform.position;
                if (offset.sqrMagnitude > Mathf.Pow(distanceToItem, 2F))
                {
                    ToggleTerminal();
                }
                
                              
                else if (Input.GetButtonUp("Tab") && player.GetComponent<FirstPersonController_Centrifugal_mp>())
                {
                    if (player.GetComponent<FirstPersonController_Centrifugal_mp>().is_paused)
                    {
                        player.GetComponent<FirstPersonController_Centrifugal_mp>().is_paused = false;
                        ToggleTerminal();
                        EventSystem.current.SetSelectedGameObject(null);
                    }
                }
                
            }

        }
    }


    //the reason you can't click on anything is because the cursor is hidden and/or locked


    //when terminal is active
        //set player is_paused to true (i.e. no movement via wasd or mouse)
            //free mouse and set visible (should be implicit in pause thing)
        //set input terminal to active
        //keep input terminal active (ready to accept text when typed)
            //even when clicking on the scroll bar or elsewhere
        //if player hits escape, exit out of terminal and unpause player

    void ToggleTerminal()
    {
        if (!TerminalActive)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f; //this makes everything transparent
                canvasGroup.blocksRaycasts = true; //this prevents the UI element to receive input events
            }
        }
        else
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0.5f; //this makes everything transparent
                canvasGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
            }
        }

        TerminalActive = !TerminalActive;
    }

    public void AdjustSliderForLight(float value)
    {
        Spotlight.intensity = LightSlider.value * 2.5f;
    }

    

}
