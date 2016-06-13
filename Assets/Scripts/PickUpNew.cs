using UnityEngine;
using System.Collections;

public class PickUpNew : MonoBehaviour {

    public int distanceToItem;
    public GameObject player;

    public Transform onhand;

    bool IsHolding = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
	void FixedUpdate()
	{
		Collect();

		//calculate velocity of held object each engine update
		//TODO add max speed to thrown objects
		objectVelocity = (onhand.position - locale) / Time.deltaTime;
		locale = onhand.position;

        //if isholding and child 0 tag = canpickup
        //set child 0 transform to onhand.position
        if(IsHolding && gameObject.transform.GetChild(0).gameObject.tag == "CanPickUp")
        {
            gameObject.transform.GetChild(0).position = onhand.position;
            gameObject.transform.GetChild(0).rotation = onhand.parent.parent.rotation;

        }
    }

    void Collect()
    {


        if (Input.GetButtonUp("Use") && !IsHolding)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            

            if (Physics.Raycast(ray, out hit, distanceToItem))
            {
                if (hit.collider.gameObject.tag == "CanPickUp")
                {
                    IsHolding = true;
                    

                    GameObject hitObject = hit.collider.gameObject;
                    hitObject.transform.parent = gameObject.transform;
                    hitObject.transform.SetAsFirstSibling();

                    hitObject.GetComponent<Rigidbody>().useGravity = false;
                    hitObject.transform.position = onhand.position;

                }

            }


        }
        
        //FIXME objects can be placed below the map and be lost
          //consequence of turning on kinematic
          //fix: keep kinematic off & implement vvv
        //TODO hover objects in front of camera


        else if (Input.GetButtonUp("Use") && IsHolding) // This will release the object 
        {
            IsHolding = false;
            Transform hitObject_transform = gameObject.transform.GetChild(0);




            hitObject_transform.gameObject.GetComponent<Rigidbody>().velocity = velocity;
            hitObject_transform.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            hitObject_transform.gameObject.GetComponent<Rigidbody>().useGravity = true;

            hitObject_transform.transform.parent = null;
            hitObject_transform = null;

            
            
        }



    }
}
