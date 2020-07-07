using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGen
{
    public static Texture2D ColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D HeightMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);
        Color[] colourMap = new Color[width * height];

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                colourMap[j * width + i] = Color.Lerp(Color.black, Color.white, noiseMap[i, j]);
            }
        }

        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TemperatureMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);
        Color[] colourMap = new Color[width * height];

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                colourMap[j * width + i] = Color.Lerp(Color.blue, Color.red, noiseMap[i, j]);
            }
        }

        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D MoistureMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);
        Color[] colourMap = new Color[width * height];

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                colourMap[j * width + i] = Color.Lerp(Color.yellow, Color.cyan, noiseMap[i, j]);
            }
        }

        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }
}
