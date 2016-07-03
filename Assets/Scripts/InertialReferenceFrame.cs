using UnityEngine;
using System.Collections;

public class InertialReferenceFrame : MonoBehaviour {

    private float rotational_period;
    public Vector3 RotationalAxis;

    public float radius;
    public float GravityAtRadius=9.81F;

    public bool rotation;

    public Transform player;
    

    // Use this for initialization
    void Start () {
        rotational_period = 2 * Mathf.PI * Mathf.Sqrt(radius / GravityAtRadius);
        //amount of seconds required for a complete rotation
        //so 360/rotation_period is the number of degrees rotated every second
    }

    // Update is called once per frame
    void Update()
    {
        if(rotation)
            transform.Rotate(RotationalAxis*(360/rotational_period)*Time.deltaTime); //rotates RotationalAxis*360/rotational_period degrees per second around z axis
        
        ConstantForce[] grav_objects = FindObjectsOfType(typeof(ConstantForce)) as ConstantForce[];

        Vector3 centrifugal_force;

        foreach (ConstantForce obj in grav_objects)
        {
            Vector3 pos = obj.gameObject.transform.position;
            //X WILL be the same
            //offset 
            centrifugal_force = new Vector3 (0, pos.y - transform.position.y, pos.z - transform.position.z);
            if(obj.tag != "Player"){
                obj.force = centrifugal_force * GravityAtRadius / radius;
            }
        }


        //player.rotation = Quaternion.Euler(-90-Mathf.Atan2((player.position.y - transform.position.y), (player.position.z - transform.position.z)) * Mathf.Rad2Deg, 0, 0);

        //^^^
        //player.eulerAngles = new Vector3(player.eulerAngles.x, player.eulerAngles.y, player.eulerAngles.z);

        //centrifugal_force = new Vector3(0, player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z);
        //player.GetComponent<Rigidbody>().AddForce(centrifugal_force * GravityAtRadius / radius);
        //player.gameObject.GetComponent<ConstantForce>().relativeForce = (centrifugal_force * player.gameObject.GetComponent<Rigidbody>().mass);
    }


}
