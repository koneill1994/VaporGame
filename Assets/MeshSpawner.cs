﻿using UnityEngine;
using System.Collections;

public class MeshSpawner : MonoBehaviour {

    public int width;           //width and height of mesh to make  (make vector2 later for simplicity's sake)
    public int height;          //never mind vector 2 contains floats not ints

    public bool GenerateFlat;

    public float length;        //length of the cylinder
    public float radius;        //radius of the cylinder
    public int VertexSep;       //separation between vertices (i.e. resolution of the terrain)

    public float heightscale;   // perlin noise gives values [0,1], this multiplies to get a height

    public float NoiseScale;    //multiplies to scale the noise; larger number is finer noise
    public Vector2 NoiseOffset; //randomize values to get different noisemaps; basically translates the noisemap presented to get different noise

	[Range(0,1)]
	public float persistance;
	public float lacunarity;


    public float[] octaves;

    private Mesh mesh;
    
    void Start () {
        if (VertexSep < 1) VertexSep = 1;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        if (GenerateFlat)
        {
            mesh.vertices = MakeVertices(width, height);
            mesh.uv = MakeUVs(mesh.vertices, width, height);
            mesh.triangles = MakeTriangles(mesh.vertices, width, height);
        }
        else
        {
            mesh.vertices = MakeVerticesCylinder(length, radius, VertexSep);
            mesh.uv = MakeUVsCylinder(mesh.vertices, length, radius);
            mesh.triangles = MakeTrianglesCylinder(mesh.vertices, length, radius);
            Debug.Log(mesh.vertices.Length);
        }

        GetComponent<MeshCollider>().sharedMesh = mesh;
        mesh.RecalculateNormals();

    }

    /*
     OK so here's how this goes
     
        PHASE 1
        
        make and spawn a mesh with an arbitrary number of coordinates
        //done!

        PHASE 2
        
        make and spawn a mesh of arbitrary size, with height determined by a procgen noise algorithm
        //done!

        PHASE 3

        make and spawn a mesh of arbitrary size, which has a simplex noise esque landscape (periodic), curved around the inside of a cylinder
        //done!

        PHASE 4

        split it up into sections (i.e. chunks) with differing levels of detail for each
            performance enhancements
                -only nearby chunks have total detail
                    -more distant chunks are less detailed, so fewer polygons to render && multithreading (hopefully) so we don't have to wait for all of them to render to begin

        issues:
        - knit cylinder together at the top
            - periodic perlin noise?
        - can't add in a scale factor smaller than one
            - noise just keeps getting smaller and smaller (we want big noise)
            - figure out why that is
        - performance enhancements
            -generate it all in the same loop


		PHASE 5

		procedural clouds
			basically have a smaller radius cylinder a distance above the surface
				render it double-sided
				
				set the texture to a perlin-noise derived texture
					if height less than n, set alpha to zero
						else alpha rises linearly with height
					increase offset with Time.time to animate it
						for each octave, increase offset at different rates to make it look like clouds are forming and dispersing


                   */

    void OnDrawGizmos()
    {
        if (mesh != null)
        {
            foreach (Vector3 v in mesh.vertices)
            {
                Gizmos.DrawIcon(v, "dot.png");
            }
        }
    }


    Vector3[] MakeVertices(int width, int height)
    {
        Vector3[] list = new Vector3[width*height];
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                float h = 0;
                //h = Mathf.PerlinNoise(NoiseOffset.x + ((float)i / (float)width) * NoiseScale, NoiseOffset.y + ((float)j / (float)height) * NoiseScale) * heightscale;
				h = PerlinOctavesNew(i, j);
                list[i * height + j] = new Vector3(i, h, j);
            }
        }
        return list;
    }

    Vector2[] MakeUVs(Vector3[] verts, int width, int height)
    {
        Vector2[] uvs = new Vector2[verts.Length];
        for(int n=0; n<verts.Length; n++)
        {
            //i * height + j = num
            // i = (num-j)/
            uvs[n] = new Vector2(n%width, Mathf.Floor(n/width));
        }

        return uvs;
    }


    //this returns a list of triangles (mesh.triangles) given a CARTESIAN LIST OF POINTS WITH THE GIVEN WIDTH AND HEIGHT
    //if you use it on something else and ruin your whole day, thats your problem
    int[] MakeTriangles(Vector3[] verts, int width, int height)
    {
        int[] Triangles = new int[(width-1)*(height-1)*2*3];
        //for width*height number of points
        //there will be (width-1)*(height-1) number of squares between those points
        //each square is made up of 2 triangles
        //each triangle is specified by 3 integers

        int m = 0;
        for (int i = 0; i < width-1; i++)
        {
            for (int j = 0; j < height-1; j++)
            {
                //for each square, make two triangles
                //the top one
                //    [(i,j),(i+1,j),(i+1,j+1)]
                //    
                Triangles[m++]= i * height + j;
                Triangles[m++] = (i + 1) * height + (j + 1);
                Triangles[m++] = (i + 1) * height + j;

                //the bottom one
                //    [(i,j),(i,j+1),(i+1,j+1)]
                Triangles[m++] = i * height + j;
                Triangles[m++] = i * height + (j+1);
                Triangles[m++] = (i + 1) * height + (j + 1);

            }
        }
        return Triangles;
    }

    //we don't need to read in the values as args here i think (?)
    Vector3[] MakeVerticesCylinder(float length, float radius, float VertexSep)
    {
        int width = Mathf.CeilToInt(2*Mathf.PI*radius/VertexSep);   //wrong axis may cause something wonky, we'll see heh
        int height = Mathf.CeilToInt(length / VertexSep);
        Debug.Log(new Vector2(width, height));

        Vector3[] list = new Vector3[width * height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float h = 0;
                h = PerlinOctaves(i, j);
                list[i * height + j] = RadialToCartesian(j*length/height, i*360/width, radius, h);
            }
        }
        return list;
    }

    int[] MakeTrianglesCylinder(Vector3[] verts, float length, float radius)
    {
        int width = Mathf.CeilToInt(2 * Mathf.PI * radius / VertexSep);   //wrong axis may cause something wonky, we'll see heh
        int height = Mathf.CeilToInt(length / VertexSep);
        int[] Triangles = new int[width * height * 2 * 3];
        //for width*height number of points
        //there will be (width-1)*(height-1) number of squares between those points
        //each square is made up of 2 triangles
        //each triangle is specified by 3 integers

        int m = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height - 1; j++)
            {
                int i_ = i % (width - 1);
                int ip_ = (i + 1) % (width - 1);

                //for each square, make two triangles
                //the top one
                //    [(i,j),(i+1,j),(i+1,j+1)]
                //    
                Triangles[m++] = i * height + j;
                Triangles[m++] = (i + 1) % (width) * height + (j + 1);
                Triangles[m++] = (i + 1) % (width) * height + j;

                //the bottom one
                //    [(i,j),(i,j+1),(i+1,j+1)]
                Triangles[m++] = i * height + j;
                Triangles[m++] = i * height + (j + 1);
                Triangles[m++] = (i + 1) % (width) * height + (j + 1);

            }
        }
        return Triangles;
    }

    Vector2[] MakeUVsCylinder(Vector3[] verts, float length, float radius)
    {
        int width = Mathf.CeilToInt(2 * Mathf.PI * radius / VertexSep);   //wrong axis may cause something wonky, we'll see heh
        int height = Mathf.CeilToInt(length / VertexSep);
        Vector2[] uvs = new Vector2[verts.Length];
        for (int n = 0; n < verts.Length; n++)
        {
            uvs[n] = new Vector2(n % width, Mathf.Floor(n / width));
        }

        return uvs;
    }


    float PerlinOctaves(int x, int y)
    {
        float output = 0;
        foreach(float m in octaves)
        {
            float i = 0.5F+NoiseOffset.x + ((float)x) * NoiseScale / m;
            float j = 0.5F+NoiseOffset.y + ((float)y) * NoiseScale / m;
            //Debug.Log(new Vector2(i, j));
            //Debug.Log(Mathf.PerlinNoise(i, j) * heightscale / m);
            output += Mathf.PerlinNoise(i,j) * heightscale * m;
            //Remember: perlin noise always returns 0.5 for integer arguments
        }
        //Debug.Log(output);
        return output;
    }

	//a new version based off the "procedural landmass" tutorial on youtube
	float PerlinOctavesNew(int x, int y){
		float amplitude = 1;
		float frequency = 1F;
		float output = 0;
		float max = 0;
		foreach(float m in octaves){
			float sampleX = (x) / NoiseScale * frequency + NoiseOffset.x;
			float sampleY = (y) / NoiseScale * frequency + NoiseOffset.y;
			float perlin = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
			output += perlin * amplitude;

			max += amplitude;
			amplitude *= persistance;
			frequency *= lacunarity;
		}
		return output/max;
	}


                                     //z      //x          //radius     //y
    public Vector3 RadialToCartesian(float l, float theta, float radius, float height)
    {
        //given a point within a cylinder
        //gives you the xyz coords to put that point in world space
        //assuming length is down X axis
        Vector3 loc = new Vector3(0, 0, 0);
        loc.x = l;
        loc.y = Mathf.Cos(Mathf.Deg2Rad * theta) * (radius - height / 2);
        loc.z = Mathf.Sin(Mathf.Deg2Rad * theta) * (radius - height / 2);

        return loc;
    }


}
