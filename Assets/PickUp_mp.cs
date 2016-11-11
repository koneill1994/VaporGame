using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PickUp_mp : NetworkBehaviour
{

    public int distanceToItem;
    public GameObject player;
    public GameObject hitObject;
    public Transform onhand;
    public Vector3 locale;
    public Vector3 objectVelocity;

    public float ThrowForce;

    bool IsHolding = false;

    // Use this for initialization
    void Start()
    {
        locale = onhand.position;
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        //calculate velocity of held object each engine update
        //TODO add max speed to thrown objects
        objectVelocity = (onhand.position - locale) / Time.deltaTime;
        locale = onhand.position;

        if (IsHolding)
        {
            hitObject.GetComponent<Rigidbody>().AddForce((onhand.position - hitObject.transform.position) * 300);
            hitObject.GetComponent<Rigidbody>().rotation = onhand.rotation;
            hitObject.GetComponent<Rigidbody>().drag = 15;
            hitObject.GetComponent<Rigidbody>().angularDrag = 15;
        }

    }
    //splitting the step into the different update types fixed the "input not registering" issue, not sure why

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetButtonUp("Use") && !IsHolding)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, distanceToItem))
            {
                if (hit.collider.gameObject.tag == "CanPickUp")
                {
                    hitObject = hit.collider.gameObject;
                    //Debug.Log(hitObject.tag);

                    IsHolding = true;
                }

            }

        }

        //FIXME objects can be placed below the map and be lost

        //TODO add angular velocity to object in same way (make it look more realistic)

        //TODO drop the cube if it gets too far away (so if it gets stuck behind a fence and you walk a mile away, it wont shoot towards you when you jump

        //you can climb up vertical walls by holding a cube under you, against the wall, jumping on it, and pulling upwards
        //TODO figure out a way to disable that?

        else if (Input.GetButtonUp("Use") && IsHolding) // This will release the object 
        {
            IsHolding = false;
            hitObject.GetComponent<Rigidbody>().drag = 0;
            hitObject.GetComponent<Rigidbody>().angularDrag = .05f;
        }

        if (Input.GetButtonUp("Fire1") && IsHolding) // This will release the object 
        {
            //Debug.Log("Throw");
            IsHolding = false;
            hitObject.GetComponent<Rigidbody>().drag = 0;
            hitObject.GetComponent<Rigidbody>().angularDrag = .05f;

            hitObject.GetComponent<Rigidbody>().AddForce(onhand.forward * ThrowForce);

        }

    }
}
