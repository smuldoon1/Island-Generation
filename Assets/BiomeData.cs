using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeData
{
    public float normalizedHeight;
    public float actualHeight;
    public float temperature;
    public float actualTemperature { get { return temperature * 80f - 30f; } }
    public Color colour;

    public BiomeData()
    {
        normalizedHeight = 0f;
        actualHeight = 0f;
        temperature = 0f;
        colour = new Color();
    }

    new public string ToString()
    {
        return "Height: " + normalizedHeight.ToString("0.00") + " (" + actualHeight.ToString("0.00") + "m)\n" +
            "Temperature: " + temperature.ToString("0.00") + " (" + actualTemperature.ToString("0.0") + "°C)\n" +
            "Biome Colour: " + colour.ToString();
    }
}
