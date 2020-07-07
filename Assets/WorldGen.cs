using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DrawMode { NoiseMap, ColourMap, TemperatureMap, MoistureMap, IslandMap, ShadedIslandMap }

public class WorldGen : MonoBehaviour
{
    public Terrain[] terrains;

    public int seed;

    [Range(1,2048)]
    public int mapWidth;
    [Range(1,2048)]
    public int mapHeight;
    [Range(1,512)]
    public float mapScaling;
    [Range(1,8)]
    public int octaves;
    [Range(0,1)]
    public float persistence;
    public float lacunarity;
    public Vector2 offset;

    public Texture2D islandMask;

    public DrawMode drawMode;

    public bool updateOnValidate;

    public void GenerateMap()
    {
        Map noiseMap = new Map(Noise.Generate2DNoiseMap(seed, mapWidth, mapHeight, mapScaling, octaves, persistence, lacunarity, offset));
        Map moistureMap = new Map(Noise.Generate2DNoiseMap(seed - 1, mapWidth, mapHeight, mapScaling, octaves, persistence-0.1f, lacunarity-0.2f, offset));
        Map temperatureMap = new Map(Noise.Generate2DNoiseMap(seed - 2, mapWidth, mapHeight, mapScaling + 40, octaves - 1, persistence - 0.2f, lacunarity - 0.3f, offset));

        temperatureMap = ~(!noiseMap * 0.4f) * (temperatureMap * 0.6f);

        if (noiseMap != null)
        {
            Color[] colourMap = ColourMap(~noiseMap, terrains, false);
            Color[] islandMap = ColourMap(~noiseMap - !(MapImage(islandMask) + 0.35f), terrains, false);
            Color[] shadedIsland = ColourMap(~noiseMap - !(MapImage(islandMask) + 0.35f), terrains, true);

            DisplayMap map = FindObjectOfType<DisplayMap>();
            switch (drawMode)
            {
                case DrawMode.NoiseMap:
                    map.DrawTexture(TextureGen.HeightMap(noiseMap.value));
                    break;
                case DrawMode.ColourMap:
                    map.DrawTexture(TextureGen.ColourMap(colourMap, mapWidth, mapHeight));
                    break;
                case DrawMode.TemperatureMap:
                    map.DrawTexture(TextureGen.TemperatureMap(temperatureMap.value));
                    break;
                case DrawMode.MoistureMap:
                    map.DrawTexture(TextureGen.MoistureMap(moistureMap.value));
                    break;
                case DrawMode.IslandMap:
                    map.DrawTexture(TextureGen.ColourMap(islandMap, mapWidth, mapHeight));
                    break;
                case DrawMode.ShadedIslandMap:
                    map.DrawTexture(TextureGen.ColourMap(shadedIsland, mapWidth, mapHeight));
                    break;
            }
            /*
            GameObject.Find("Plane").AddComponent(typeof(MeshFilter));
            MeshFilter mesh = GameObject.Find("Plane").GetComponent<MeshFilter>();
            mesh.mesh.vertices = new Vector3[16384];

            for (int i = 0; i < mesh.sharedMesh.vertices.Length; i++)
            {
                Vector2Int coords = new Vector2Int(Mathf.FloorToInt(i / noiseMap.width), i % noiseMap.width);
                mesh.sharedMesh.vertices[i].y = 20 * noiseMap.value[coords.x, coords.y];
                mesh.sharedMesh.vertices[i].x = coords.x;
                mesh.sharedMesh.vertices[i].y = coords.y;
            }*/
        }
    }

    Color[] ColourMap(Map map, Terrain[] terrain, bool shading)
    {
        return ColourMap(map, null, terrain, shading);
    }

    Color[] ColourMap(Map map, Map resourceMap, Terrain[] terrain, bool shading)
    {
        Color[] colourMap = new Color[map.width * map.height];
        for (int j = 0; j < map.height; j++)
        {
            for (int i = 0; i < map.width; i++)
            {
                float currentHeight = map.value[i, j];
                for (int k = 0; k < terrains.Length; k++)
                {
                    if (currentHeight <= terrains[k].height)
                    {
                        if (shading)
                        {
                            colourMap[j * map.width + i] = Color.Lerp(terrains[k].colour, terrains[k].shadedColour, (1.1f - map.value[i,j]) * 1.5f);
                        }
                        else
                        {
                            colourMap[j * map.width + i] = terrains[k].colour;
                        }
                        break;
                    }
                }
            }
        }
        return colourMap;
    }

    Map MapImage(Texture2D image)
    {
        int width = image.width;
        int height = image.height;

        Map map = new Map(width, height);

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                map.SetValue(i, j, image.GetPixel(i, j).grayscale);
            }
        }
        return map;
    }
}

[System.Serializable]
public struct Terrain
{
    public string name;
    public float height;
    public Color colour;
    public Color shadedColour;
}