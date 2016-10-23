using UnityEngine;
using System.Collections;

public class AntiGravityZone : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        Rigidbody other_rb = other.gameObject.GetComponent<Rigidbody>();
        other_rb.useGravity = false;

    }

    void OnTriggerExit(Collider other)
    {
        Rigidbody other_rb = other.gameObject.GetComponent<Rigidbody>();
        other_rb.useGravity = true;
    }

}
