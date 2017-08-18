using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    public Terrain terrain;
    public int size = 513;
    public int seed = 1;
    public float roughness = 0.5f;

    private float[,] dataArray;

	// Use this for initialization
	void Start () {
        UnityEngine.Random.InitState(seed);
        GenerateDiamondSquareArray();

    }

    // Update is called once per frame
    void Update () {
		
	}

    private void GenerateDiamondSquareArray()
    {
        if (Math.Abs(Math.Log(size - 1, 2) % 1) > (Double.Epsilon * 100))
            throw new Exception("Invalid terrain size (2^n+1)");

        //if ((roughness < 0.0f) || (roughness > 1.0f))
        //    throw new Exception("Invalid roughness factor. Must be between 0 & 1");


        dataArray = new float[size, size];

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

        Debug.Log("Terrain data generation completed");

        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        if (terrain == null)
            return;

        if (terrain.terrainData.heightmapResolution != size)
            terrain.terrainData.heightmapResolution = size;

        terrain.terrainData.SetHeights(0, 0, dataArray);

        Debug.Log("Terrain heightmap set");

        AddTerrainTextures();
    }

    private void AddTerrainTextures()
    {
        if (terrain == null)
            return;

        TerrainData terrainData = terrain.terrainData;

        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight,
                                        terrainData.alphamapLayers];
        for (int y=0; y<terrainData.alphamapHeight; y++)
        {
            for (int x=0; x<terrainData.alphamapWidth; x++)
            {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)y / (float)terrainData.alphamapHeight;
                float x_01 = (float)x / (float)terrainData.alphamapWidth;

                // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapHeight), Mathf.RoundToInt(x_01 * terrainData.heightmapWidth));

                // Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
                Vector3 normal = terrainData.GetInterpolatedNormal(y_01, x_01);

                // Calculate the steepness of the terrain
                float steepness = terrainData.GetSteepness(y_01, x_01);

                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[terrainData.alphamapLayers];

                // CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT

                // Texture[0] has constant influence
                splatWeights[0] = 0.5f;

                // Texture[1] is stronger at lower altitudes
                splatWeights[1] = Mathf.Clamp01((terrainData.heightmapHeight - height));

                // Texture[2] stronger on flatter terrain
                // Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
                // Subtract result from 1.0 to give greater weighting to flat surfaces
                splatWeights[2] = 1.0f - Mathf.Clamp01(steepness * steepness / (terrainData.heightmapHeight / 5.0f));

                // Texture[3] increases with height but only on surfaces facing positive Z axis 
                splatWeights[3] = height * Mathf.Clamp01(normal.z);

                // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                float z = splatWeights.Sum();

                // Loop through each terrain texture
                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {

                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;


                    // Assign this point to the splatmap array
                    splatmapData[x, y, i] = splatWeights[i];
                }
            }
        }

        // Finally assign the new splatmap to the terrainData:
        terrainData.SetAlphamaps(0, 0, splatmapData);

        Debug.Log("Terrain textures applied");

    }
}
