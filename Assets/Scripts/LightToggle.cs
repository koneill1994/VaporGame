using UnityEngine;
using System.Collections;

//this is largely a test to make sure a method exists to change things via activateable buttons
//if you ever need a button to do something, come here  and reference this code

public class LightToggle : MonoBehaviour {

    public int distanceToItem;
    public GameObject player;

    bool IsActive = true;


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonUp("Use"))
        {
            //use button is pressed
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //see if there's a toggleable light in front of the player, and toggle it
            if (Physics.Raycast(ray, out hit, distanceToItem))
            {
                if (hit.collider.gameObject.tag == "ToggleableLight")
                {
                    Toggle_Light();
                }
            }
        }
	}


    //TODO
    //Add a script to update to change the IsActive value based on the intensity of the light
    //in other words, update the bool value to reflect the true status of the light
    //added flexibility and all that
    
    void Toggle_Light()
    {
        Debug.Log("Toggling Light");
        GameObject light_source = gameObject.transform.GetChild(0).gameObject;
        if (IsActive)
        {
            //turn the light off if its on
            light_source.GetComponent<Light>().intensity = 0;
            IsActive = false;
        }
        else
        {
            //turn the light on if its off
            light_source.GetComponent<Light>().intensity = 1;
            IsActive = true;
        }
    }
}
