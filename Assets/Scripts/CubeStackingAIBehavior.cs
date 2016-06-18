using UnityEngine;
using System.Collections;

public class CubeStackingAIBehavior : MonoBehaviour {

    public enum StackerGoal {PickTargetObject, PathToObject, PathToPile, FreezeWhenSeen}
    public StackerGoal CurrentGoal;
    public GameObject Goal_Object;
    public Transform onhand;
    public Transform pile;
    public bool OnlyMoveWhenUnseen = false;
    public bool RecalcTargetDistsEveryFrame = false;
    
    public float PickUpRange = 10;
    public float pile_size = 10;

    private NavMeshAgent agent;
    private bool IsHolding = false;
    private float base_speed;

    // Use this for initialization
    void Start () {
        CurrentGoal = StackerGoal.PickTargetObject;
        agent = GetComponent<NavMeshAgent>();
        base_speed = agent.speed;
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(agent.speed);
        
        if (gameObject.GetComponent<Renderer>().isVisible && OnlyMoveWhenUnseen)
        {
            //Debug.Log("IS VISIBLE");
            agent.speed = 0.0f;
            agent.velocity = Vector3.zero;
            CurrentGoal = StackerGoal.FreezeWhenSeen;
            agent.destination = pile.position;
        }
        else
        {
            //NOT VISIBLE
            agent.speed = base_speed;
        }
        

        if (CurrentGoal == StackerGoal.PickTargetObject)
        {
            //chooses the closest game object to the agent
            Goal_Object = GetClosestObjectViaPathing(GameObject.FindGameObjectsWithTag("CanPickUp")).gameObject;
            //TODO only choose an object if it can be pathed to
            CurrentGoal = StackerGoal.PathToObject;
        }

        else if (CurrentGoal == StackerGoal.PathToObject)
        {
            if(RecalcTargetDistsEveryFrame)
                Goal_Object = GetClosestObjectViaPathing(GameObject.FindGameObjectsWithTag("CanPickUp")).gameObject;
            //THIS IS COSTLY (framrate drop when active)
            //figure out a better method

            agent.destination = Goal_Object.transform.position;
            Vector3 offset = transform.position - Goal_Object.transform.position;
            if (offset.magnitude < PickUpRange)
            {
                IsHolding = true;
                CurrentGoal = StackerGoal.PathToPile;
            }
        }
        /*
        else if (CurrentGoal == StackerGoal.PickUpObject)
        {
            
            CurrentGoal = StackerGoal.PathToPile;
        }
        */
        else if (CurrentGoal == StackerGoal.PathToPile)
        {
            if (IsHolding)
            {
                Goal_Object.GetComponent<Rigidbody>().AddForce((onhand.position - Goal_Object.transform.position) * 300);
                Goal_Object.GetComponent<Rigidbody>().rotation = onhand.rotation;
                Goal_Object.GetComponent<Rigidbody>().drag = 15;
                Goal_Object.GetComponent<Rigidbody>().angularDrag = 15;
            }
            Vector3 offset = transform.position - pile.position;
            agent.destination = pile.position;
            if (offset.magnitude < PickUpRange)
            {
                IsHolding = false;
                Goal_Object.GetComponent<Rigidbody>().drag = .5f;
                Goal_Object.GetComponent<Rigidbody>().angularDrag = .05f;
                CurrentGoal = StackerGoal.PickTargetObject;
                //Goal_Object.tag = "InPile";
            }
        }
        else if (CurrentGoal == StackerGoal.FreezeWhenSeen)
        {
            Vector3 offset = transform.position - pile.position;
            if (offset.magnitude < PickUpRange)
            {
                CurrentGoal = StackerGoal.PickTargetObject;
                //Goal_Object.tag = "InPile";
            }
        }


        for (int i = 0; i < agent.path.corners.Length - 1; i++)
        {
            Debug.DrawLine(agent.path.corners[i], agent.path.corners[i + 1]);
        }
    }

    //THINGS TO ADD
    // behavior to freeze, go invisible, chase player when unseen (i.e. weeping angel style) if player  gets too close
    // more robust pile system, picking up objects
    // ability to wander around randomly, searching for cubes, & only path to them & bring them back if they are discovered while searching
    //more or less stringent "pile coherence" standards based on number of cubes in pile & number in "bring to pile" queue
    //ignore light cubes, or ability to extinguish them when adding them to a pile ( to prevent lighting glitches from concentrating light sources in one room)
        //player can re-ignite light cubes (or only have one lit at a time?)
    //drop objects if they get too far away ?  (need to add to pickupnew as well)
        //if so, a way to stop ai from getting stuck in a pick-up, drop loop?

    
    Transform GetClosestObject(GameObject[] objects)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject potential_target in objects)
        {
            Vector3 offset = potential_target.transform.position - pile.position;
            Vector3 directionToTarget = potential_target.transform.position - currentPos;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                if (offset.magnitude > pile_size)
                {
                    bestTarget = potential_target.transform;
                    closestDistanceSqr = dSqrToTarget;
                }
            }
        }
        return bestTarget;
    }



    Transform GetClosestObjectViaPathing(GameObject[] objects)
    {
        Transform bestTarget = null;
        float closestDistancePath = Mathf.Infinity;
        NavMeshPath path = new NavMeshPath();
        foreach (GameObject potential_target in objects)
        {
            if (NavMesh.CalculatePath(transform.position, potential_target.transform.position, NavMesh.AllAreas, path))
            {
                float PathDistanceToTarget = PathLength(path);
                Vector3 offset = potential_target.transform.position - pile.position;
                Debug.Log(potential_target.name);
                Debug.Log(PathDistanceToTarget);
                if (PathDistanceToTarget < closestDistancePath)
                {
                    if (offset.magnitude > pile_size)
                    {
                    bestTarget = potential_target.transform;
                        closestDistancePath = PathDistanceToTarget;
                    }
                }
            }
        }
        return bestTarget;
    }


    float PathLength(NavMeshPath path)
    {
        if (path.corners.Length < 2)
            return 0;

        Vector3 previousCorner = path.corners[0];
        float lengthSoFar = 0.0F;
        int i = 1;
        while (i < path.corners.Length)
        {
            Vector3 currentCorner = path.corners[i];
            lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
            previousCorner = currentCorner;
            i++;
        }
        return lengthSoFar;
    }
}
