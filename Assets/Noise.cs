using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateRandom2DNoiseMap(int seed, int width, int height)
    {
        float[,] noiseMap = new float[width, height];

        System.Random rng = new System.Random(seed);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                noiseMap[i, j] = Mathf.PerlinNoise(i / width, j / height);
            }
        }

        return noiseMap;
    }

    public static float[,] Generate2DNoiseMap(int seed, int width, int height, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[width, height];

        System.Random rng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rng.Next(-100000, 100000) + offset.x;
            float offsetY = rng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (width <= 0 || height <= 0 || scale <= 0)
        {
            Debug.LogWarning("Width, height and scale must be greater than 0! Aborting.");
            return null;
        }

        Vector2 heightRange = new Vector2(float.MaxValue, float.MinValue);

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                float frequency = 1;
                float amplitude = 1;
                float noiseHeight = 0;

                for (int k = 0; k < octaves; k++)
                {
                    float sampleX = i / scale * frequency + octaveOffsets[k].x;
                    float sampleY = j / scale * frequency + octaveOffsets[k].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseHeight < heightRange.x)
                {
                    heightRange.x = noiseHeight;
                }
                else if (noiseHeight > heightRange.y)
                {
                    heightRange.y = noiseHeight;
                }

                noiseMap[i, j] = noiseHeight;
            }
        }

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                noiseMap[i, j] = Mathf.InverseLerp(heightRange.x, heightRange.y, noiseMap[i, j]);
            }
        }

        return noiseMap;
    }
}
