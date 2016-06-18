using UnityEngine;
using System.Collections;

public class SpawnGrabbableOnUse : MonoBehaviour {

    public int distanceToItem;

    public GameObject SpawnObject;
    public Vector3 SpawnOffset;


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonUp("Use"))
        {
            //use button is pressed
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //see if there's a toggleable light in front of the player, and toggle it
            if (Physics.Raycast(ray, out hit, distanceToItem) && hit.collider.gameObject == gameObject)
            {
                Instantiate(SpawnObject, transform.position + SpawnOffset, Quaternion.identity);
            }
        }
    }


    void Spawn_Item()
    {
        Debug.Log("Test");
    }
}
