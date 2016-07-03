using UnityEngine;
using System.Collections;

public class SpawnGrabbableOnUse : MonoBehaviour {

    public int distanceToItem;

    public GameObject SpawnObject;
    public Vector3 SpawnOffset;

    //public string Message;

    // Use this for initialization
    void Start () {
        GUIStyle myStyle = new GUIStyle();
        /*
        myStyle.font = gameObject.GetComponenet<GUIText>().font;
        myStyle.fontSize = guiText.fontSize;

        Vector2 size = myStyle.CalcSize(new GUIContent(guiText.text));
        */
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonUp("Use"))
        {
            //use button is pressed
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, distanceToItem) && hit.collider.gameObject == gameObject)
            {
                if (transform.parent != null)
                {
                    GameObject spawned = (GameObject)Instantiate(SpawnObject, transform.position + transform.rotation*SpawnOffset, transform.rotation);
                    spawned.transform.parent = transform.parent;
                }
                else
                {
                    Instantiate(SpawnObject, transform.position + SpawnOffset, Quaternion.identity);
                }
            }
        }
    }

    //TODO make it so you don't have to run the check twice

    void OnGUI()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, distanceToItem) && hit.collider.gameObject == gameObject)
        {
            //Font.GetCharacterInfo(char ch, out characterInfo info, int messageSize = 0, FontStyle style = FontStyle.Normal);
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, Screen.width / 2, 20), "Spawn a Cube");
        }
    }

}
