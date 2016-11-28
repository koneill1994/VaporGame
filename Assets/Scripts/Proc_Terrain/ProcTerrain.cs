using UnityEngine;
using System.Collections;

public class ProcTerrain : MonoBehaviour {



    public TerrainType[] regions;



    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public Mesh CreateMesh(MapData mapdata)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = CreateVertices(mapdata.heightMap);
        

        return mesh;
    }

    public Vector3[] CreateVertices(float[,] heightmap)
    {
        Vector3[] verts = new Vector3[heightmap.GetLength(0) * heightmap.GetLength(1)];
        int vert_counter=0;
        for(int y=0; y< heightmap.GetLength(1); y++)
        {
            for(int x=0; x< heightmap.GetLength(0); x++)
            {
                verts[vert_counter++] = new Vector3(x, heightmap[x, y], y);
            }
        }
        return verts;
    }
    
	public MapData CreateMap(int width, int height, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset){
		float[,] noiseMap = new float[width, height];
		Color[] colorMap = new Color[width * height];

		System.Random prng = new System.Random(seed);

		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i=0; i<octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000)+offset.x;
			float offsetY = prng.Next(-100000, 100000)+offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		if (scale <= 0)
		{
			scale = 0.0001f;
		}

		float halfWidth = width / 2f;
		float halfHeight = height / 2f;

		for(int y=0; y< height; y++){
			for(int x=0; x< width; x++){
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for(int i=0; i<octaves; i++){
					float sampleX = (x-halfWidth) / scale*frequency + octaveOffsets[i].x;
					float sampleY = (y-halfHeight) / scale*frequency + octaveOffsets[i].y;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY)*2 -1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;

				}
				noiseMap [x, y] = noiseHeight;
                colorMap[y * height + x] = ColorFromHeight(noiseHeight);
			}
		}
		return new MapData (noiseMap, colorMap);
	}

    public Color ColorFromHeight(float height) {
        //assuming a height from 0 to 1
        foreach(TerrainType t in regions)
        {
            if (height <= t.height)
            {
                return t.color;
            }
        }
        return Color.black;
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}

public struct MapData{
	public float[,] heightMap;
	public Color[] colorMap;

	public MapData(float[,] heightMap, Color[] colorMap){
		this.heightMap = heightMap;
		this.colorMap = colorMap;
	}
}