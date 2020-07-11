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

        // Setup gradient
        Gradient gradient = new Gradient();
        gradient.mode = GradientMode.Blend;

        GradientColorKey[] colourKey = new GradientColorKey[8];
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[8];

        colourKey[0].color = RGBColour(254, 245, 255);
        colourKey[0].time = 0f;

        colourKey[1].color = RGBColour(255, 51, 223);
        colourKey[1].time = 0.12f;

        colourKey[2].color = RGBColour(2, 13, 195);
        colourKey[2].time = 0.28f;

        colourKey[3].color = RGBColour(25, 196, 236);
        colourKey[3].time = 0.47f;

        colourKey[4].color = RGBColour(185, 242, 107);
        colourKey[4].time = 0.62f;

        colourKey[5].color = RGBColour(233, 175, 11);
        colourKey[5].time = 0.75f;

        colourKey[6].color = RGBColour(242, 29, 29);
        colourKey[6].time = 0.87f;

        colourKey[7].color = RGBColour(138, 5, 5);
        colourKey[7].time = 1f;

        alphaKey[0].alpha = 1f;
        alphaKey[0].time = 0f;

        alphaKey[1].alpha = 1f;
        alphaKey[1].time = 0.12f;

        alphaKey[2].alpha = 1f;
        alphaKey[2].time = 0.28f;

        alphaKey[3].alpha = 1f;
        alphaKey[3].time = 0.47f;

        alphaKey[4].alpha = 1f;
        alphaKey[4].time = 0.62f;

        alphaKey[5].alpha = 1f;
        alphaKey[5].time = 0.75f;

        alphaKey[6].alpha = 1f;
        alphaKey[6].time = 0.87f;

        alphaKey[7].alpha = 1f;
        alphaKey[7].time = 1f;

        gradient.SetKeys(colourKey, alphaKey);

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                colourMap[j * width + i] = gradient.Evaluate(noiseMap[i, j]);
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

    static Color RGBColour(int red, int green, int blue)
    {
        return RGBColour(red, green, blue, 255);
    }

    static Color RGBColour(int red, int green, int blue, int alpha)
    {
        return new Color(red / 255f, green / 255f, blue / 255f, alpha / 255f);
    }
}
