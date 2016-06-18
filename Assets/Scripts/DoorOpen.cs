using UnityEngine;
using System.Collections;

public class DoorOpen : MonoBehaviour {

	[SerializeField] private Transform door;
	[SerializeField] private Transform doorLight;
	private bool doorOpen = false;
	// Use this for initialization
	void Start () {
		Quaternion orig = door.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate ()
	{
		RaycastHit hit;

		if (Physics.Raycast(transform.position, transform.right, out hit))
		{
			if (hit.transform.name != "Receiver" && !doorOpen) {
				Debug.Log (hit.transform.name);
				door.transform.Rotate (0, -90, 0);
				doorOpen = true;
				doorLight.GetComponent<Light>().color = Color.green;
			} else {
			}
		}
	}
}
