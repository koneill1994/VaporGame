﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class RandomCubeSpawner : NetworkBehaviour {
    


    public float radius;
    public float length;
    public int NumberOfCubes;
    
    public Vector2 MinMaxCubeWidth;   //cubes will be larger than first value and smaller than second value
    public Vector2 MinMaxCubeHeight;

    public float Offset;

    public GameObject TerrainCube;

    [SyncVar]
    public int seed;

    //OKAY NEW PLAN
    //have the server generate the seed
    //and send it to all the clients via RPC or syncvar so that they can make it themselves

    // Use this for initialization
    void Start () {
        Debug.Log("Starting Cube Spawner");
        //if (isServer)
        //{

        Random.InitState(seed);
            MinMaxCubeWidth = SanitizeMinMax(MinMaxCubeWidth);
            MinMaxCubeHeight = SanitizeMinMax(MinMaxCubeHeight);

            for (int n=0; n < NumberOfCubes; n++)
            {
                //get together all our variables used to place the cubes
                float l = Random.Range(-length / 2, length/2);  //position down the length of the cylinder
                float phi = Random.Range(0, 180);                       
                float theta = phi % 60 + Mathf.Floor(phi / 60)*120;     //position along the inner circumference (weird because we're skipping over half the space within the cylinder)
                theta += Offset;
                Vector3 scale = new Vector3(Random.Range(MinMaxCubeWidth.x, MinMaxCubeWidth.y), Random.Range(MinMaxCubeHeight.x, MinMaxCubeHeight.y), Random.Range(MinMaxCubeWidth.x, MinMaxCubeWidth.y));
                Vector3 coords = RadialToCartesian(l, theta, radius, scale.y);
                
                                
                //network instantiate
                GameObject g = (GameObject) Instantiate(TerrainCube, coords, Quaternion.Euler(theta,0,0));
                g.transform.localScale = scale;
                g.transform.parent = transform;
                

                //^^ this is throwing an error at the first one
                // but still adds it to the list -_-
                //figure out how to suppress the error so it goes through the whole thing


                Debug.Log(string.Concat("Cube number ", n, " spawned"));
            }


            
        //}
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public Vector2 SanitizeMinMax(Vector2 MinMax)
    {

        if (MinMax.x >= MinMax.y)
        {
            //make sure minimum is less than or equal to maximum
            MinMax.x = MinMax.y;
        }
        //if either are less than or equal to zero, set to 1
        MinMax.x = (MinMax.x <= 0) ? 1F : MinMax.x;
        MinMax.y = (MinMax.y <= 0) ? 1F : MinMax.y;

        return MinMax;
    }

    public Vector3 RadialToCartesian(float l, float theta, float radius, float height)
    {
        //assuming length is down X axis
        Vector3 loc = new Vector3(0, 0, 0);
        loc.x = l;
        loc.y = Mathf.Cos(Mathf.Deg2Rad * theta) * (radius-height/2);
        loc.z = Mathf.Sin(Mathf.Deg2Rad * theta) * (radius-height/2);

        return loc;
    }

    public int StringToInt(string str)
    {
        int output = 0;
        int i = 0;
        foreach (char c in str)
        {
            output += Mathf.Min(System.Convert.ToInt32(c),127) * (int)Mathf.Pow(128, i);
            i++;
        }
        return output;
    }

    public void SetSeed(string input)
    {
        seed = StringToInt(input);
    }

}
