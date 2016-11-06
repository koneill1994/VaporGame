using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class RandomCubeSpawner : NetworkBehaviour {
    
    public float radius;
    public float length;
    public int NumberOfCubes;
    
    public float MinimumCubeDimension;
    public float MaximumCubeDimension;

    public GameObject TerrainCube;


    // Use this for initialization
    void Start () {
        if (isServer)
        {
            if (MinimumCubeDimension>=MaximumCubeDimension)
            {
                //make sure minimum is less than or equal to maximum
                MinimumCubeDimension = MaximumCubeDimension;
            }
            //if either are less than or equal to zero, set to 1
            MinimumCubeDimension = (MinimumCubeDimension <= 0) ? 1F : MinimumCubeDimension;
            MaximumCubeDimension = (MaximumCubeDimension <= 0) ? 1F : MaximumCubeDimension;

            for(int n=0; n < NumberOfCubes; n++)
            {
                //get together all our variables used to place the cubes
                float l = Random.Range(-length / 2, length/2);
                float phi = Random.Range(0, 360);
                float theta = phi % 30 + Mathf.Floor(phi / 3) * 60;
                Vector3 coords = RadialToCartesian(l, theta, radius);
                Vector3 scale = new Vector3(Random.Range(MinimumCubeDimension, MaximumCubeDimension), Random.Range(MinimumCubeDimension, MaximumCubeDimension), Random.Range(MinimumCubeDimension, MaximumCubeDimension));
                                
                //network instantiate
                GameObject g = (GameObject) Instantiate(TerrainCube, coords, Quaternion.Euler(theta,0,0));
                g.transform.localScale = scale;
                g.transform.parent = transform;
                NetworkServer.Spawn(g);
                Debug.Log(n);
            }


            
        }
        //Network.Instantiate(TerrainCube, transform.position, Quaternion.Euler(0, 0, 0), 0);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public Vector3 RadialToCartesian(float l, float theta, float radius)
    {
        //assuming length is down X axis
        Vector3 loc = new Vector3(0, 0, 0);
        loc.x = l;
        loc.y = Mathf.Cos(Mathf.Rad2Deg * theta) * radius;
        loc.z = Mathf.Sin(Mathf.Rad2Deg * theta) * radius;

        return loc;
    }




}
