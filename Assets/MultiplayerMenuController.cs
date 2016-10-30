using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MultiplayerMenuController : MonoBehaviour {

    NetworkClient myClient;
    private CanvasGroup canvasGroup;
    public NetworkManager n_manager;
    private InputField server_ip;

    // Use this for initialization
    void Start () {
        canvasGroup = GetComponent<CanvasGroup>();
        server_ip = canvasGroup.gameObject.transform.Find("server_ip").GetComponent<InputField>();
	}
	
	// Update is called once per frame
	void Update () {
        if (n_manager.IsClientConnected())
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    //https://stackoverflow.com/questions/36091976/can-i-use-the-unity-networking-hlapi-without-paying-for-the-unity-multiplayer-se


    //https://docs.unity3d.com/Manual/UNetSceneObjects.html
    // ^ this is why the cylinder is gone before anything spawns

    public void CreateServer() //this code is called when a player clicks the "create server button
    {
        n_manager.networkAddress = "localhost";
        n_manager.networkPort = 7777;
        //create the server
        //join the server

        n_manager.StartHost();

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void JoinServer()
    {
        n_manager.networkAddress = server_ip.text;
        n_manager.networkPort = 7777;

        n_manager.StartClient();


        
    }

}
