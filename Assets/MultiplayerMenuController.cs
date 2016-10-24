using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class MultiplayerMenuController : MonoBehaviour {

    NetworkClient myClient;
    public CanvasGroup canvasGroup;

    // Use this for initialization
    void Start () {
        canvasGroup = GetComponent<CanvasGroup>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //https://stackoverflow.com/questions/36091976/can-i-use-the-unity-networking-hlapi-without-paying-for-the-unity-multiplayer-se

    public void CreateServer() //this code is called when a player clicks the "create server button
    {
        myClient = new NetworkClient();

        //create the server
        //join the server
        NetworkServer.Listen(7777);


        myClient.Connect("localhost", 7777);

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;


    }




}
