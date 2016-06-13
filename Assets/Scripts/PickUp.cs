using UnityEngine;
using System.Collections;

public class PickUP : MonoBehaviour {

    public int distanceToItem;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Collect();
	}

    void Collect()
    {
        if (Input.GetMouseButtonUp(19))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, distanceToItem))
            {
                if(hit.collider.gameObject.tag == "CanPickUp"){
                    Debug.Log("item hit");

                    Destroy(hit.collider.gameObject);
                }

            }


        }


    }
}
