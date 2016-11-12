using UnityEngine;
using System.Collections;

public class MeshSpawner : MonoBehaviour {

    public int width;
    public int height;




    public Vector3[] newVertices;
    public Vector2[] newUV;
    public int[] newTriangles;

    private Mesh mesh;

    // Use this for initialization
    void Start () {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        //mesh.vertices = newVertices;
        mesh.vertices = MakeVertices(width, height);
        //mesh.uv = newUV;
        mesh.triangles = MakeTriangles(mesh.vertices,width,height);



        Instantiate(mesh);
    }
	
    /*
     OK so here's how this goes
     
        PHASE 1
        
        make and spawn a mesh with an arbitrary number of coordinates
        
        PHASE 2
        
        make and spawn a mesh of arbitrary size, with height determined by a procgen simplex noise algorithm

        PHASE 3

        make and spawn a mesh of arbitrary size, which has a simplex noise esque landscape (periodic), curved around the inside of a cylinder
                   */




    void OnDrawGizmos()
    {
        if (mesh != null)
        {
            foreach (Vector3 p in mesh.vertices)
            {
                Gizmos.DrawIcon(p, "pixels.png", true);
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
                list[i * height + j] = new Vector3(i, 0, j);
            }
        }
        return list;
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
