using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class mp_TerminalMenuController : MonoBehaviour {

    public int distanceToItem;

    public Light Spotlight;
    public Slider LightSlider;

    private CanvasGroup canvasGroup;
    private bool TerminalActive = false;

    public Camera player_camera;

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
            RaycastHit hit;
            //Ray ray = GameObject.FindWithTag("client_cam").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            Ray ray = player_camera.ScreenPointToRay(Input.mousePosition);
            //see if there's a terminal in front of the player
            if (Physics.Raycast(ray, out hit, distanceToItem) && hit.collider.gameObject == gameObject)
            {
                //use button is pressed
                if (Input.GetButtonUp("Use"))
                {
                    ToggleTerminal();
                }
            }
            //make sure the terminal is close enough to be activated
            Vector3 offset = transform.position - player_camera.transform.position;
            if (offset.sqrMagnitude > Mathf.Pow(distanceToItem, 2F) && TerminalActive)
            {
                ToggleTerminal();
            }
        }
    }


    //the reason you can't click on anything is because the cursor is hidden and/or locked

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
