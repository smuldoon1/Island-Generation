using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldGen))]
public class WorldGenEditor : Editor
{
    bool advancedSettings = false;
    bool terrainOptions = false;

    WorldGen worldGen;
    BiomeData data = new BiomeData();

    private void OnSceneGUI()
    {
        Vector2 mousePos = Event.current.mousePosition;
        mousePos.y = Camera.current.pixelHeight - mousePos.y;
        Ray ray = Camera.current.ScreenPointToRay(mousePos);
        RaycastHit hit;
        data = new BiomeData();
        Debug.Log(ray);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.GetComponent<SubMesh>())
            {
                data = hit.collider.GetComponent<SubMesh>().GetDataOnMesh(hit.point.x, hit.point.z);
            }
        }
    }

    [System.Obsolete] // Bypasses warning about EditorGUILayout.ObjectField()
    public override void OnInspectorGUI()
    {
        worldGen = (WorldGen)target;

        EditorGUILayout.LabelField("Biome data: ", data.ToString(), GUILayout.MinHeight(50f));
        worldGen.seed = EditorGUILayout.IntField("Seed", worldGen.seed);

        worldGen.mapWidth = EditorGUILayout.IntSlider("Map Width", worldGen.mapWidth, 1, 2048);
        worldGen.mapHeight = EditorGUILayout.IntSlider("Map Height", worldGen.mapHeight, 1, 2048);
        if (advancedSettings = EditorGUILayout.Foldout(advancedSettings, "Advanced Settings"))
        {
            worldGen.mapScaling = EditorGUILayout.Slider("Scaling", worldGen.mapScaling, 1, 512);
            worldGen.octaves = EditorGUILayout.IntSlider("Octaves", worldGen.octaves, 1, 8);
            worldGen.persistence = EditorGUILayout.Slider("Persistence", worldGen.persistence, 0, 1);
            worldGen.lacunarity = EditorGUILayout.Slider("Lacunarity", worldGen.lacunarity, 0, 16);
            worldGen.offset = EditorGUILayout.Vector2Field("Offset", worldGen.offset);
            worldGen.islandMask = (Texture2D)EditorGUILayout.ObjectField("Island Mask", worldGen.islandMask, typeof(Texture2D));
            worldGen.tempMask = (Texture2D)EditorGUILayout.ObjectField("Temperature Mask", worldGen.tempMask, typeof(Texture2D));
            worldGen.biomeGuide = (Texture2D)EditorGUILayout.ObjectField("Biome Guide", worldGen.biomeGuide, typeof(Texture2D));
            worldGen.material = (Material)EditorGUILayout.ObjectField("Terrain Material", worldGen.material, typeof(Material));
        }
        worldGen.drawMode = (DrawMode)EditorGUILayout.EnumPopup("Draw Mode", worldGen.drawMode);

        worldGen.updateOnValidate = EditorGUILayout.Toggle("Auto Generate", worldGen.updateOnValidate);
        worldGen.randomiseOnStart = EditorGUILayout.Toggle("Randomise on Start", worldGen.randomiseOnStart);

        if (GUI.changed)
        {
            if (worldGen.updateOnValidate && Application.isPlaying)
            {
                worldGen.GenerateMap();
            }
        }

        if (!Application.isPlaying) GUI.enabled = false;
        if (GUILayout.Button("Generate World")) worldGen.GenerateMap();
        if (!Application.isPlaying) GUI.enabled = true;

        if (GUILayout.Button("Randomise Seed"))
        {
            worldGen.RandomiseSeed();
            if (worldGen.updateOnValidate)
            {
                worldGen.GenerateMap();
            }
        }

        if (terrainOptions = EditorGUILayout.Foldout(terrainOptions, "Terrain Options"))
        {
            if (worldGen.terrains.Length == 0)
            {
                EditorGUILayout.LabelField("Start by adding a terrain.");
            }
            for (int i = 0; i < worldGen.terrains.Length; i++)
            {
                GUILayout.BeginHorizontal();
                {
                    worldGen.terrains[i].name = EditorGUILayout.TextField("Terrain Name", worldGen.terrains[i].name);
                    if (GUILayout.Button("^") && i != 0)
                    {
                        Terrain tempTerrain = worldGen.terrains[i];
                        worldGen.terrains[i] = worldGen.terrains[i - 1];
                        worldGen.terrains[i - 1] = tempTerrain;
                    }
                    if (GUILayout.Button("v") && i != worldGen.terrains.Length - 1)
                    {
                        Terrain tempTerrain = worldGen.terrains[i];
                        worldGen.terrains[i] = worldGen.terrains[i + 1];
                        worldGen.terrains[i + 1] = tempTerrain;
                    }
                    if (GUILayout.Button("-"))
                    {
                        worldGen.terrains = RemoveTerrain(i ,worldGen.terrains);
                        break;
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (i != worldGen.terrains.Length - 1)
                {
                    worldGen.terrains[i].height = EditorGUILayout.Slider("Height", worldGen.terrains[i].height,
                        i != 0 ? worldGen.terrains[i - 1].height : 0,
                        i != worldGen.terrains.Length - 1 ? worldGen.terrains[i + 1].height : 1);
                }
                else
                {
                    worldGen.terrains[i].height = EditorGUILayout.Slider("Height", worldGen.terrains[i].height, 1, 1);
                }
                worldGen.terrains[i].colour = EditorGUILayout.ColorField(worldGen.terrains[i].colour);
                worldGen.terrains[i].shadedColour = EditorGUILayout.ColorField(worldGen.terrains[i].shadedColour);
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add Terrain"))
            {
                worldGen.terrains = AddTerrain(worldGen.terrains);
            }
        }

        //SortTerrains();
    }

    void SortTerrains()
    {
        bool sorted = false;
        while (!sorted)
        {
            sorted = true;
            for (int i = 0; i < worldGen.terrains.Length - 1; i++)
            {
                if (worldGen.terrains[i].height > worldGen.terrains[i + 1].height)
                {
                    Terrain tempTerrain = worldGen.terrains[i];
                    worldGen.terrains[i] = worldGen.terrains[i + 1];
                    worldGen.terrains[i + 1] = tempTerrain;
                    sorted = false;
                }
            }
        }
    }

    Terrain[] RemoveTerrain(int index, Terrain[] array)
    {
        List<Terrain> list = new List<Terrain>(array);
        list.RemoveAt(index);
        return list.ToArray();
    }

    Terrain[] AddTerrain(Terrain[] array)
    {
        List<Terrain> list = new List<Terrain>(array);
        list.Add(new Terrain());
        return list.ToArray();
    }
}
