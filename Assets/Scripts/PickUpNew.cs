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
    void Update()
    {
        Collect();

        //if isholding and child 0 tag = canpickup
        //set child 0 transform to onhand.position
        if(IsHolding && gameObject.transform.GetChild(0).gameObject.tag == "CanPickUp")
        {
            gameObject.transform.GetChild(0).position = onhand.position;
            gameObject.transform.GetChild(0).rotation = onhand.parent.parent.rotation;

        }
        //trying to figure out how to give the held object momentum when dropped
        //i.e. give it the parent's velocity instead of just setting them to zero
        //Debug.Log(gameObject.transform.parent.gameObject.GetComponent<Rigidbody>().velocity);
        //Debug.Log(gameObject.transform.parent.gameObject.name);
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


            //Rigidbody hitObject_rb = hitObject_transform.gameObject.GetComponent<Rigidbody>();
            //hitObject_rb.isKinematic = false;

            // Debug.Log(hitObject_rb.name);
            //  Debug.Log(hitObject_rb.isKinematic);

            hitObject_transform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            hitObject_transform.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            hitObject_transform.gameObject.GetComponent<Rigidbody>().useGravity = true;

            hitObject_transform.transform.parent = null;
            hitObject_transform = null;

            
            
        }



    }
}
