using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DrawMode { NoiseMap, ColourMap, TemperatureMap, MoistureMap, IslandMap, ShadedIslandMap, BiomeMap }

public class WorldGen : MonoBehaviour
{
    public Terrain[] terrains;
    public Material material;

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
    public Texture2D tempMask;
    public Texture2D biomeGuide;

    public DrawMode drawMode;

    public bool updateOnValidate;
    public bool randomiseOnStart;

    Map noiseMap;
    Map temperatureMap;
    Map moistureMap;

    public delegate void WorldCreation();
    public static event WorldCreation GeneratedWorld;

    private void Awake()
    {
        if (randomiseOnStart)
        {
            RandomiseSeed();
        }
        GenerateMap();
    }

    public void RandomiseSeed()
    {
        seed = Random.Range(int.MinValue, int.MaxValue);
    }

    public void GenerateMap()
    {
        noiseMap = new Map(Noise.Generate2DNoiseMap(seed, mapWidth, mapHeight, mapScaling, octaves, persistence, lacunarity, offset));
        moistureMap = new Map(Noise.Generate2DNoiseMap(seed - 1, mapWidth, mapHeight, mapScaling, octaves, persistence-0.1f, lacunarity-0.2f, offset));
        temperatureMap = new Map(Noise.Generate2DNoiseMap(seed, mapWidth, mapHeight, mapScaling, octaves, persistence, 1f, offset));

        temperatureMap = ~(temperatureMap * moistureMap);
        temperatureMap = !~temperatureMap;
        noiseMap -= (!MapImage(islandMask) - 0.35f) - MapImage(tempMask);
        temperatureMap -= MapImage(tempMask);
        noiseMap += MapImage(tempMask) * 0.5f;

        if (noiseMap != null)
        {
            Color[] colourMap = ColourMap(~noiseMap, terrains, false);
            Color[] islandMap = ColourMap(~noiseMap - !(MapImage(islandMask) + 0.35f), terrains, false);
            Color[] shadedIsland = ColourMap(noiseMap, terrains, true);
            Color[] biomeMap = BiomeMap(noiseMap, temperatureMap);

            switch (drawMode)
            {
                case DrawMode.NoiseMap:
                    DrawTexture(TextureGen.HeightMap(noiseMap.value));
                    break;
                case DrawMode.ColourMap:
                    DrawTexture(TextureGen.ColourMap(colourMap, mapWidth, mapHeight));
                    break;
                case DrawMode.TemperatureMap:
                    DrawTexture(TextureGen.TemperatureMap(temperatureMap.value));
                    break;
                case DrawMode.MoistureMap:
                    DrawTexture(TextureGen.MoistureMap(moistureMap.value));
                    break;
                case DrawMode.IslandMap:
                    DrawTexture(TextureGen.ColourMap(islandMap, mapWidth, mapHeight));
                    break;
                case DrawMode.ShadedIslandMap:
                    DrawTexture(TextureGen.ColourMap(shadedIsland, mapWidth, mapHeight));
                    break;
                case DrawMode.BiomeMap:
                    DrawTexture(TextureGen.ColourMap(biomeMap, mapWidth, mapHeight));
                    break;
            }
        }

        GeneratedWorld();
    }

    Color[] ColourMap(Map map, Terrain[] terrain, bool shading)
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

    Color[] BiomeMap(Map heightMap, Map temperatureMap)
    {
        Color[] colourMap = new Color[heightMap.width * heightMap.height];
        for (int j = 0; j < heightMap.height; j++)
        {
            for (int i = 0; i < heightMap.width; i++)
            {
                float currentHeight = heightMap.value[i, j];
                for (int k = 0; k < terrains.Length; k++)
                {
                    if (currentHeight <= terrains[k].height)
                    {
                        colourMap[j * heightMap.width + i] = GetBiomeColour(heightMap.value[i, j], temperatureMap.value[i, j]);
                        break;
                    }
                }
            }
        }
        return colourMap;
    }

    Color GetBiomeColour(float height, float temperature)
    {
        height = Mathf.Clamp(height, 0f, 0.99f);
        temperature = Mathf.Clamp(temperature, 0f, 0.99f);
        return biomeGuide.GetPixel(Mathf.RoundToInt(temperature * biomeGuide.width), Mathf.RoundToInt(height * biomeGuide.height));
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

    public float GetVertexHeight(int x, int z)
    {
        float height = noiseMap.value[x, z];
        return height;
    }

    public float GetTerrainTemperature(int x, int z)
    {
        float temperature = temperatureMap.value[x, z];
        return temperature;
    }

    public Color GetTerrainColour(int x, int z)
    {
        Color colour = GetBiomeColour(noiseMap.value[x, z], temperatureMap.value[x, z]);
        return colour;
    }

    public void DrawTexture(Texture2D texture)
    {
        material.mainTexture = texture;
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