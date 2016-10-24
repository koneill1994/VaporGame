using UnityEngine;
using System.Collections;

public class CustomGravity : MonoBehaviour {

    public Vector3 Gravity_Vector;
    public float gravity_acceleration = 9.81F;

	// Use this for initialization
	void Start () {
        /*
        if (GetComponent<ConstantForce>() != null)
        {
            GetComponent<ConstantForce>().force = Gravity_Vector;
        }
        */
        

        
	}
	
	// Update is called once per frame
	void Update () {
        Rigidbody rb = GetComponent<Rigidbody>();

        Gravity_Vector = Vector3.down * gravity_acceleration * rb.mass;

        rb.AddRelativeForce(Gravity_Vector);

    }
}
