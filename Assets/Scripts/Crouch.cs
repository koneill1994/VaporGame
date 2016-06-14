using UnityEngine;
using System.Collections;

public class Crouch : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		while (Input.GetButton ("Crouch")) 
		{
			gameObject.transform.localScale = new Vector3(0, .5F, 0);
		}
	}
}
