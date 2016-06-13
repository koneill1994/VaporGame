using UnityEngine;
using System.Collections;

public class PickUpNew : MonoBehaviour {

    public int distanceToItem;
    public GameObject player;
	public GameObject hitObject;
    public Transform onhand;
    public Vector3 locale;
    Vector3 objectVelocity;

    bool IsHolding = false;

    // Use this for initialization
    void Start()
    {
        locale = onhand.position;
    }

    void FixedUpdate()
    {
        //calculate velocity of held object each engine update
        //TODO add max speed to thrown objects
        objectVelocity = (onhand.position - locale) / Time.deltaTime;
        locale = onhand.position;

		//if isholding and child 0 tag = canpickup
		//set child 0 transform to onhand.position
		if (IsHolding)
		{
			hitObject.GetComponent<Rigidbody>().AddForce((onhand.position-hitObject.transform.position)*100);
			//TODO motion dampening
		}

    }
    //splitting the step into the different update types fixed the "input not registering" issue, not sure why

    // Update is called once per frame
    void Update()
	{

        if (Input.GetButtonUp("Use") && !IsHolding)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, distanceToItem))
            {
                if (hit.collider.gameObject.tag == "CanPickUp")
                {
					hitObject = hit.collider.gameObject;
					Debug.Log(hitObject.tag);

                    IsHolding = true;
                }

            }
		
        }

        //FIXME objects can be placed below the map and be lost
                        
        //TODO add angular velocity to object in same way (make it look more realistic)

        else if (Input.GetButtonUp("Use") && IsHolding) // This will release the object 
        {
            IsHolding = false;
        }

    }
}
