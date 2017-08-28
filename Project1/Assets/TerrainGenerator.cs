using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    public int size = 513;
    public int segmentSize = 64;
    public int seed = 2;
    public float roughness = 0.5f;
    public float maxHeight = 20.0f;
    public float waterLevel = 3.0f;

    private float[,] heightMap = null;

	// Use this for initialization
	void Start () {

        if ((size-1) % segmentSize != 0)
            throw new Exception("Segment size must be a factor of (Size - 1)");


        UnityEngine.Random.InitState(seed);
        heightMap = GenerateDSHeightMap();


        int seg_num = (size - 1) / segmentSize;
        for (int seg_x=0; seg_x < seg_num; seg_x++)
        {
            for (int seg_z = 0; seg_z < seg_num; seg_z++)
            {
                GameObject t = new GameObject();
                t.name = "Segment (" + seg_x + ", " + seg_z + ")";
                t.transform.parent = this.gameObject.transform;
                MeshFilter tMesh = t.gameObject.AddComponent<MeshFilter>();
                tMesh.sharedMesh = this.CreateTerrainMesh(heightMap, seg_x, seg_z);
                MeshRenderer renderer = t.gameObject.AddComponent<MeshRenderer>();
                renderer.material.shader = Shader.Find("Unlit/CubeShader");
            }
        }

    }

    // Update is called once per frame
    void Update () {
		// *****************************************************************************************************************************
		// ************************FOR DEBUGGING PURPOSES ONLY!*************************************************************************
		// *****************************************************************************************************************************
		if (Input.GetKeyDown ("f")) {
			this.seed = (int)(UnityEngine.Random.value*100);
			Start();
		}
		// *****************************************************************************************************************************
		// *****************************************************************************************************************************
		// *****************************************************************************************************************************
	}

    public float? getHeight(int x, int z)
    {
        if (x > (size - 1) || z > (size - 1))
            return null;
        return heightMap[x, z] * maxHeight;
    }

    private float[,] GenerateDSHeightMap()
    {
        if (Math.Abs(Math.Log(size - 1, 2) % 1) > (Double.Epsilon * 100))
            throw new Exception("Invalid terrain size (2^n+1)");

        //if ((roughness < 0.0f) || (roughness > 1.0f))
        //    throw new Exception("Invalid roughness factor. Must be between 0 & 1");


        float[,] dataArray = new float[size, size];

        dataArray[0, 0] = 0.0f;
        dataArray[size - 1, 0] = 0.0f;
        dataArray[0, size - 1] = 0.0f;
        dataArray[size - 1, size - 1] = 0.0f;


        float h = roughness;

        for (int sideLength = size-1; sideLength >= 2; sideLength /= 2, h /= 2)
        {
            int halfSide = sideLength / 2;

            // Squares
            for (int x=0; x < size-1; x+=sideLength)
            {
                for (int y=0; y < size-1; y += sideLength)
                {
                    float val = dataArray[x, y];
                    val += dataArray[x + sideLength, y];
                    val += dataArray[x, y + sideLength];
                    val += dataArray[x + sideLength, y + sideLength];

                    val /= 4.0f;

                    float rnd = (UnityEngine.Random.value * 2.0f * h) - h;
                    val = Mathf.Clamp01(val + rnd);

                    dataArray[x + halfSide, y + halfSide] = val;
                }
            }

            // Diamonds
            for (int x=0; x < size-1; x+= halfSide)
            {
                for (int y=(x+halfSide) % sideLength; y < size-1; y+=sideLength)
                {
                    float val = dataArray[(x - halfSide + size - 1) % (size - 1), y];
                    val += dataArray[(x + halfSide) % (size - 1), y];
                    val += dataArray[x, (y + halfSide) % (size - 1)];
                    val += dataArray[x, (y - halfSide + size - 1) % (size - 1)];

                    val /= 4.0f;

                    float rnd = (UnityEngine.Random.value * 2.0f * h) - h;
                    val = Mathf.Clamp01(val + rnd);


                    dataArray[x, y] = val;

                    if (x == 0) dataArray[size - 1, y] = val;
                    if (y == 0) dataArray[x, size - 1] = val;

                }
            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                if (dataArray[x, z] <= (waterLevel / maxHeight)) dataArray[x, z] = (waterLevel / maxHeight);
            }
        }

        Debug.Log("Terrain data generation completed");

        return dataArray;

    }


    private Mesh CreateTerrainMesh(float[,] heightMap, int seg_x, int seg_z)
    {
        Mesh mesh = new Mesh();
        mesh.name = "TerrainMesh ("+seg_x+", "+seg_z+")";

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> indices = new List<int>();
        List<Color> colours = new List<Color>();

        int width = this.segmentSize;
        int length = this.segmentSize;

        Debug.Log("Mesh w:" + width + ", l:" + length);

        for (int x=(seg_x*width); x < (seg_x+1)*width; x++)
        {
            for (int z=(seg_z*length); z < (seg_z+1)*length; z++)
            {
                float y = heightMap[x, z] * this.maxHeight;
                vertices.Add(new Vector3(x, y, z));
                uvs.Add(new Vector2(0.0f, 0.0f));
                normals.Add(Vector3.up);
                colours.Add(this.getTerrainColour(y));

                y = heightMap[x, z+1] * this.maxHeight;
                vertices.Add(new Vector3(x, y, z+1.0f));
                uvs.Add(new Vector2(0.0f, 1.0f));
                normals.Add(Vector3.up);
                colours.Add(this.getTerrainColour(y));

                y = heightMap[x+1, z+1] * this.maxHeight;
                vertices.Add(new Vector3(x+1.0f, y, z+1.0f));
                uvs.Add(new Vector2(1.0f, 1.0f));
                normals.Add(Vector3.up);
                colours.Add(this.getTerrainColour(y));

                y = heightMap[x+1, z] * this.maxHeight;
                vertices.Add(new Vector3(x+1.0f, y, z));
                uvs.Add(new Vector2(1.0f, 0.0f));
                normals.Add(Vector3.up);
                colours.Add(this.getTerrainColour(y));

                int baseIndex = vertices.Count - 4;

                indices.Add(baseIndex);
                indices.Add(baseIndex + 1);
                indices.Add(baseIndex + 2);

                indices.Add(baseIndex);
                indices.Add(baseIndex + 2);
                indices.Add(baseIndex + 3);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();

        if (normals.Count == vertices.Count)
        {
            Debug.Log("Normals added to mesh.");
            mesh.normals = normals.ToArray();
        }

        if (uvs.Count == vertices.Count)
        {
            Debug.Log("Texture UVs added to mesh.");
            mesh.uv = uvs.ToArray();
        }

        if (colours.Count == vertices.Count)
        {
            Debug.Log("Colours added to mesh.");
            mesh.colors = colours.ToArray();
        }

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();


        return mesh;
        
    }

    Color getTerrainColour(float height)
    {
        // Water
        if (height <= waterLevel)
        {
            return new Color(0.0f, 0.5f, 1.0f, 1.0f);
        }
        // Snowy Mountaintops
        if (height >= (0.85f * this.maxHeight))
        {
            return new Color(0.96f,0.96f,0.96f,1.0f);
        }
        // Grass
        else if (height <= (0.4f * this.maxHeight))
        {
            return new Color(0.0f,0.74f,0.0f,1.0f);
        }
        // Dirt/Mountain slope
        else
        {
            return new Color(0.57f,0.44f,0.44f,1.0f);
        }
    }
}
