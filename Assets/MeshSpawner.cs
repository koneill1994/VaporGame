using UnityEngine;
using System.Collections;

public class MeshSpawner : MonoBehaviour {

    public int width;
    public int height;

    public float heightscale;

    public float NoiseScale;
    public Vector2 NoiseOffset;

    private Mesh mesh;

    // Use this for initialization
    void Start () {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = MakeVertices(width, height);
        mesh.uv = MakeUVs(mesh.vertices,width,height);
        mesh.triangles = MakeTriangles(mesh.vertices,width,height);

        mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = mesh;

    }

    /*
     OK so here's how this goes
     
        PHASE 1
        
        make and spawn a mesh with an arbitrary number of coordinates
        //done!

        PHASE 2
        
        make and spawn a mesh of arbitrary size, with height determined by a procgen simplex noise algorithm
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

    int[] MakeTriangles(Vector3[] verts, int width, int height)
    {
        //this returns a list of triangles (mesh.triangles) given a CARTESIAN LIST OF POINTS WITH THE GIVEN WIDTH AND HEIGHT
        //if you use it on something else and ruin your whole day, thats your problem

        int[] Triangles = new int[(width-1)*(height-1)*2*3];
        //for width*height number of points
        //there will be (width-1)*(height-1) number of squares between those points
        //each square is made up of 2 triangles
        //each triangle is specified by 3 integers

        int m = 0;  //i can't be bothered to do it without a counter at the moment
        //do the math later
        for (int i = 0; i < width-1; i++)
        {
            for (int j = 0; j < height-1; j++)
            {
                //so for each square, make two triangles
                //the top one
                //    [(i,j),(i+1,j),(i+1,j+1)]
                //    
                Triangles[m]= i * height + j;
                m++;
                Triangles[m] = (i + 1) * height + (j + 1);
                m++;
                Triangles[m] = (i + 1) * height + j;
                m++;

                //the bottom one
                //    [(i,j),(i,j+1),(i+1,j+1)]
                Triangles[m] = i * height + j;
                m++;
                Triangles[m] = i * height + (j+1);
                m++;
                Triangles[m] = (i + 1) * height + (j + 1);
                m++;

            }
        }
        //remember, triangle list is a list of positions in the vertices list
        return Triangles;
    }


    int VertNumFromCoords(int i, int j)
    {
        //not sure if ill need this yet
        //i * height + j = num
        //
        return 0;
    }

}
