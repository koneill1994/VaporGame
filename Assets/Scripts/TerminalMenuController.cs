using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TerminalMenuController : MonoBehaviour {

    public int distanceToItem;

    public Light Spotlight;
    public Slider LightSlider;

    private CanvasGroup canvasGroup;
    private bool TerminalActive = false;

    public Camera player_camera;

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
    void Update () {                
        RaycastHit hit;
        //Ray ray = GameObject.FindWithTag("client_cam").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        Ray ray = player_camera.ScreenPointToRay(Input.mousePosition);
        //see if there's a terminal in front of the player
        if (Physics.Raycast(ray, out hit, distanceToItem) && hit.collider.gameObject == gameObject)
        {
            Debug.Log("RaycastHit");
            //use button is pressed
            if (Input.GetButtonUp("Use"))
            {
                ToggleTerminal();
                Debug.Log("HIT");
            }
        }
        Vector3 offset = transform.position - Camera.main.transform.position;
        if (offset.sqrMagnitude > Mathf.Pow(distanceToItem,2F) && TerminalActive)
        {
            ToggleTerminal();
        }
    }


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
