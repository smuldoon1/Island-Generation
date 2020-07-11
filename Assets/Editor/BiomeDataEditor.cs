using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BiomeDataEditor : EditorWindow
{
    BiomeData data = new BiomeData();

    [MenuItem("Window/Biome Data")]
    public static void ShowWindow()
    {
        GetWindow(typeof(BiomeDataEditor));
    }
}
