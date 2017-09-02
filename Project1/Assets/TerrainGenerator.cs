using System;
using System.Collections.Generic;
using UnityEngine;



// Class for generation of basic terrain, based on the Diamond-Square Algorithm.
// To create terrain, attach this script to an empty GameObject.
public class TerrainGenerator : MonoBehaviour {

    // The length and width of the terrain.
    public int size = 513;

    // The length and width of each segment of the terrain.
    public int segmentSize = 64;

    // The seed used by the Random number generator.
    public int seed = 2;

    // Determines how rough/mountainous the terrain is.
    public float roughness = 0.5f;

    // The height of the highest point of the terrain.
    public float maxHeight = 20.0f;

    // The height of the water level.
    public float waterLevel = 3.0f;

    // The heightmap used to create the terrain. Generated at start.
    private float[,] heightMap = null;

    // Shader used by the terrain.
    public Shader shader;

    // The Sun pointlight.
    public PointLight pointLight;

    // Use this for initialization
    void Start () {

        if ((size-1) % segmentSize != 0)
            throw new Exception("Segment size must be a factor of (Size - 1)");

        seed = System.DateTime.Now.Millisecond;
        UnityEngine.Random.InitState(seed);

        // Generate new heightmap.
        heightMap = GenerateDSHeightMap();

        // Creates and renders terrain GameObject by separating terrain into a grid
        // of child GameObjects, each forming a segment of the terrain mesh.
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
                renderer.material.shader = shader;
            }
        }

    }

    // Update is called once per frame
    void Update () {
        // Gets each of the MeshRenderers attached to each segment within the terrain GameObject.
        MeshRenderer[] children = this.gameObject.GetComponentsInChildren<MeshRenderer>();

        // Updates each MeshRenderer with the new colour and position of the Sun.
        foreach (MeshRenderer i in children)
        {
            i.material.SetColor("_PointLightColor", this.pointLight.color);
            i.material.SetVector("_PointLightPosition", this.pointLight.GetWorldPosition());
        }
    }

    // Returns the height of the terrain at a given xz position.
    // Returns null if position is outside the terrain's bounds.
    public float? getHeight(int x, int z)
    {
        if (x > (size - 1) || z > (size - 1))
            return null;
        return heightMap[x, z] * maxHeight;
    }

    // Generates a 2D array of floats between 0 and 1 using the Diamond-Square Algorithm
    // to form a heightmap for the terrain.
    private float[,] GenerateDSHeightMap()
    {
        // Throws exception if the terrain size isn't valid.
        if (Math.Abs(Math.Log(size - 1, 2) % 1) > (Double.Epsilon * 100))
            throw new Exception("Invalid terrain size (2^n+1)");

        float[,] heightArray = new float[size, size];

        // Corners of the heightmap are initialised to 0.
        heightArray[0, 0] = 0.0f;
        heightArray[size - 1, 0] = 0.0f;
        heightArray[0, size - 1] = 0.0f;
        heightArray[size - 1, size - 1] = 0.0f;


        float h = roughness;

        // Calculate the height in the middle of each square and diamond, with sideLength 
        // equal to the side length of each sqare, which halfs each iteration, till all
        // postions of the heightmap have been calculated.
        for (int sideLength = size-1; sideLength >= 2; sideLength /= 2, h /= 2)
        {

            int halfSide = sideLength / 2;

            // Calculate the height in the centre of each square
            for (int x=0; x < size-1; x+=sideLength)
            {
                for (int z=0; z < size-1; z += sideLength)
                {
                    // Calculate the average height of the corners of the square
                    float val = heightArray[x, z];
                    val += heightArray[x + sideLength, z];
                    val += heightArray[x, z + sideLength];
                    val += heightArray[x + sideLength, z + sideLength];

                    val /= 4.0f;

                    // Add random change to the calculated height value,
                    // proportional to the roughness factor
                    float rnd = (UnityEngine.Random.value * 2.0f * h) - h;
                    val = Mathf.Clamp01(val + rnd);

                    heightArray[x + halfSide, z + halfSide] = val;
                }
            }

            // Calculate the height in the centre of each diamond
            for (int x=0; x < size-1; x+= halfSide)
            {
                for (int z=(x+halfSide) % sideLength; z < size-1; z+=sideLength)
                {
                    // Calculate the average height of the corners of the diamond
                    float val = heightArray[(x - halfSide + size - 1) % (size - 1), z];
                    val += heightArray[(x + halfSide) % (size - 1), z];
                    val += heightArray[x, (z + halfSide) % (size - 1)];
                    val += heightArray[x, (z - halfSide + size - 1) % (size - 1)];

                    val /= 4.0f;

                    // Add random change to the calculated height value,
                    // proportional to the roughness factor
                    float rnd = (UnityEngine.Random.value * 2.0f * h) - h;
                    val = Mathf.Clamp01(val + rnd);


                    heightArray[x, z] = val;

                    // If calculating initial diamond values at sides of the terrain
                    if (x == 0) heightArray[size - 1, z] = val;
                    if (z == 0) heightArray[x, size - 1] = val;

                }
            }
        }

        // Changes all heights below water level to the water level.
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                if (heightArray[x, z] <= (waterLevel / maxHeight)) heightArray[x, z] = (waterLevel / maxHeight);
            }
        }

        Debug.Log("Terrain data generation completed");

        return heightArray;

    }

    // Creates the Mesh for a segment of the terrain, based on height values given by the heightmap.
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

        // For grid square in the segment, create a quad primitive
        for (int x=(seg_x*width); x < (seg_x+1)*width; x++)
        {
            for (int z=(seg_z*length); z < (seg_z+1)*length; z++)
            {
                // Create vertex for bottom-left corner
                float y = heightMap[x, z] * this.maxHeight;
                vertices.Add(new Vector3(x, y, z));
                uvs.Add(new Vector2(0.0f, 0.0f));
                normals.Add(Vector3.up);
                colours.Add(this.getTerrainColour(y));

                // Create vertex for top-left corner
                y = heightMap[x, z+1] * this.maxHeight;
                vertices.Add(new Vector3(x, y, z+1.0f));
                uvs.Add(new Vector2(0.0f, 1.0f));
                normals.Add(Vector3.up);
                colours.Add(this.getTerrainColour(y));

                // Create vertex for top-right corner
                y = heightMap[x+1, z+1] * this.maxHeight;
                vertices.Add(new Vector3(x+1.0f, y, z+1.0f));
                uvs.Add(new Vector2(1.0f, 1.0f));
                normals.Add(Vector3.up);
                colours.Add(this.getTerrainColour(y));

                // Create vertex for bottom-right corner
                y = heightMap[x+1, z] * this.maxHeight;
                vertices.Add(new Vector3(x+1.0f, y, z));
                uvs.Add(new Vector2(1.0f, 0.0f));
                normals.Add(Vector3.up);
                colours.Add(this.getTerrainColour(y));

                int baseIndex = vertices.Count - 4;

                // Add arrangement of vertices to form mesh triangles
                indices.Add(baseIndex);
                indices.Add(baseIndex + 1);
                indices.Add(baseIndex + 2);

                indices.Add(baseIndex);
                indices.Add(baseIndex + 2);
                indices.Add(baseIndex + 3);
            }
        }

        // Create mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();

        if (normals.Count == vertices.Count)
        {
            mesh.normals = normals.ToArray();
        }

        if (uvs.Count == vertices.Count)
        {
            mesh.uv = uvs.ToArray();
        }

        if (colours.Count == vertices.Count)
        {
            mesh.colors = colours.ToArray();
        }

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();


        return mesh;
        
    }

    // Returns the colour for a terrain vertex based on its height.
    Color getTerrainColour(float height)
    {
        // Water (blue)
        if (height <= waterLevel)
        {
            return new Color(0.0f, 0.5f, 1.0f, 1.0f);
        }
        // Snowy Mountaintops (light grey)
        else if (height >= (0.85f * this.maxHeight))
        {
            return new Color(0.96f,0.96f,0.96f,1.0f);
        }
        // Sand (yellow)
        else if (height <= waterLevel + (0.02f * this.maxHeight))
        {
            return new Color(0.80f, 0.62f, 0.0f, 1.0f);
        }
        // Grass (green)
        else if (height <= waterLevel + (0.1f * this.maxHeight))
        {
            return new Color(0.28f,0.52f,0.23f,1.0f);
        }
        // Dirt/Mountain slope (brown)
        else
        {
            return new Color(0.57f,0.44f,0.44f,1.0f);
        }
    }
}
