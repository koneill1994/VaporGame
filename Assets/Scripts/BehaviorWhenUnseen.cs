using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class BehaviorWhenUnseen : MonoBehaviour {

    public float speed = 3.0F;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        CharacterController controller = GetComponent<CharacterController>();
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        if (gameObject.GetComponent<Renderer>().isVisible)
        {
            //Debug.Log("IS VISIBLE");
        }
        else
        {
            GameObject[] main_cam = GameObject.FindGameObjectsWithTag("MainCamera");
            if (main_cam.Length==1)
            {
                Vector3 targetDir = main_cam[0].transform.position - transform.position;
                //Debug.Log("NOT VISIBLE");
                Vector3 newDir = Vector3.RotateTowards(gameObject.transform.forward, targetDir, 4.0f, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDir);

                //this is to lock the y axis
                // i.e. so if you jump on top of it, it wont turn up to look at you
                Vector3 eulerAngles = transform.rotation.eulerAngles;
                eulerAngles = new Vector3(0, eulerAngles.y, 0);
                transform.rotation = Quaternion.Euler(eulerAngles);

                //this is the code to make it follow you

                controller.SimpleMove(forward * speed);

            }
        }
	}
}
