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


    public float rot;

    // Use this for initialization
    void Start()
    {
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
    void Update()
    {
        Debug.Log("1: " + MainCamera.transform.rotation.eulerAngles);
        //Debug.Log("1" + MainCamera.transform.rotation.eulerAngles);
        SkyCamera.transform.rotation = MainCamera.transform.rotation;

        //SkyCamera.transform.Rotate(SkyCamera.transform.rotation.eulerAngles * -1);
        //reset rotation to zero every time

        //Vector3 mc = MainCamera.transform.rotation.eulerAngles;
        //freaks out when you go to far from zero

        //relates to gimbal lock?
        //https://docs.unity3d.com/Manual/QuaternionAndEulerRotationsInUnity.html
        //


        //SkyCamera.transform.Rotate(mc);
        
        //SkyBoxRotation = new Vector3(0,0,rot * Time.deltaTime%360);

        //SkyCamera.transform.rotation = MainCamera.transform.rotation*Quaternion.Euler(SkyBoxRotation);


        //SkyCamera.transform.Rotate(SkyBoxRotation);
        //^^does not rotate from zero, rotates from wherever the thing is already pointing
        
        //SkyCamera.transform.rotation = Quaternion.Euler(SkyBoxRotation);
        //^this is bad, it causes the jittering
        //all rotation needs to be done via transform.Rotate


    }
}
