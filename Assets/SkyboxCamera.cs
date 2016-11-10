using UnityEngine;
using System.Collections;

public class SkyboxCamera : MonoBehaviour
{
    

    // set the main camera in the inspector
    public Camera MainCamera;

    // set the sky box camera in the inspector
    public Camera SkyCamera;

    // the additional rotation to add to the skybox
    // can be set during game play or in the inspector
    public Vector3 SkyBoxRotation;

    public GameObject dir_light;

    public float rot;

    private Quaternion sunangle; //set to rotation at beginning

    // Use this for initialization
    void Start()
    {
        sunangle = dir_light.transform.rotation;
        if (SkyCamera.depth >= MainCamera.depth)
        {
            Debug.Log("Set skybox camera depth lower " +
                " than main camera depth in inspector");
        }
        if (MainCamera.clearFlags != CameraClearFlags.Nothing)
        {
            Debug.Log("Main camera needs to be set to dont clear" +
                "in the inspector");
        }
    }

    // if you need to rotate the skybox during gameplay
    // rotate the skybox independently of the main camera
    public void SetSkyBoxRotation(Vector3 rotation)
    {
        this.SkyBoxRotation = rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Debug.Log("1: " + MainCamera.transform.rotation.eulerAngles);

        //determine how much the cylinder has rotated mod 360 since the start
        SkyBoxRotation = new Vector3(rot * Time.time % 360,0, 0);

        //add that rotation to the rotation due to the player's camera moving
        SkyCamera.transform.rotation = Quaternion.Euler(SkyBoxRotation)*MainCamera.transform.rotation;
        // ^ NB: Quaternion multiplication is not commutative (a*b != b*a)

        //rotate the sun around as well
        dir_light.transform.rotation = Quaternion.Euler(-SkyBoxRotation) * sunangle;

    }
}
