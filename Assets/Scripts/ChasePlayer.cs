// MoveTo.cs
using UnityEngine;
using System.Collections;

public class ChasePlayer : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent agent;
    private float base_speed;
    public bool OnlyMoveWhenUnseen;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        base_speed = agent.speed;    
    }

    void Update()
    {
        agent.destination = player.transform.position;
        
        if (gameObject.GetComponent<Renderer>().isVisible && OnlyMoveWhenUnseen)
        {
            //Debug.Log("IS VISIBLE");
            agent.speed = 0.0f;
            agent.velocity = Vector3.zero;
        }
        else
        {
            //NOT VISIBLE
            agent.speed = base_speed;
            GameObject[] main_cam = GameObject.FindGameObjectsWithTag("MainCamera");
            if (main_cam.Length == 1)
            {
                

            }
        }


    }
}
