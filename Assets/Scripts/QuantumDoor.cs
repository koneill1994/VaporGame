using UnityEngine;
using System.Collections;

public class QuantumDoor : MonoBehaviour {
	public Collider trigger;
	public GameObject door;

	// Use this for initialization
	void Start () {
		trigger = GameObject.Find ("FPSController").GetComponent<CharacterController> ();
		door = GameObject.Find ("frontDoor");
		door.SetActive (false);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
	}

	void OnTriggerEnter(Collider trigger)
	{
		Debug.Log("Entered");
		if (!GameObject.Find("Terrain").GetComponent<Renderer>().isVisible)
		{
			door.SetActive (true);
		}
	}
}
