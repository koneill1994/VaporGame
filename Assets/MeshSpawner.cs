using UnityEngine;
using System.Collections;

public class MeshSpawner : MonoBehaviour {

    public int width;           //width and height of mesh to make  (make vector2 later for simplicity's sake)
    public int height;

    public float heightscale;   // perlin noise gives values [0,1], this multiplies to get a height

    public float NoiseScale;    //multiplies to scale the noise; larger number is finer noise
    public Vector2 NoiseOffset; //randomize values to get different noisemaps; basically translates the noisemap presented to get different noise

    private Mesh mesh;
    
    void Start () {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = MakeVertices(width, height);
        mesh.uv = MakeUVs(mesh.vertices,width,height);
        mesh.triangles = MakeTriangles(mesh.vertices,width,height);
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


                   */



    Vector3[] MakeVertices(int width, int height)
    {
        Vector3[] list = new Vector3[width*height];
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                float h = 0;
                h = Mathf.PerlinNoise(NoiseOffset.x + (float)i / (float)width * NoiseScale, NoiseOffset.y + (float)j / (float)height * NoiseScale) * heightscale;
                list[i * height + j] = new Vector3(i, h, j);
                //Debug.Log(h);
                //Debug.Log(new Vector2((float)i / (float)width * (float)NoiseScale, (float)j / (float)height * (float)NoiseScale));
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

}
