using UnityEngine;
using System.Collections;

public class TerminalMenuController : MonoBehaviour {

    public int distanceToItem;

    private CanvasGroup canvasGroup;
    private bool TerminalActive = false;

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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //see if there's a terminal in front of the player
        if (Physics.Raycast(ray, out hit, distanceToItem) && hit.collider.gameObject == gameObject)
        {
            //use button is pressed
            if (Input.GetButtonUp("Use"))
            {
                ToggleTerminal();
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

}
