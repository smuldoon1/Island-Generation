using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMesh : MonoBehaviour
{
    public int x;
    public int z;
    public Vector2Int position { get { return new Vector2Int(x, z); } set { x = value.x; z = value.y; } }

    public BiomeData[,] biomeData;

    public MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    public void Awake()
    {
        meshFilter = (MeshFilter)gameObject.AddComponent(typeof(MeshFilter));
        meshRenderer = (MeshRenderer)gameObject.AddComponent(typeof(MeshRenderer));
    }

    public void SetMaterial(Material material)
    {
        meshRenderer.material = material;
    }

    public void SetBiomeDataSize(int width, int breadth)
    {
        biomeData = new BiomeData[width, breadth];
    }

    public BiomeData GetDataOnMesh(float xPosition, float zPosition)
    {
        return biomeData[x * MeshGenerator.maxMeshSize + Mathf.FloorToInt(xPosition % MeshGenerator.maxMeshSize), z * MeshGenerator.maxMeshSize + Mathf.FloorToInt(zPosition % MeshGenerator.maxMeshSize)];
    }

    public BiomeData GetDataOnMesh(int x, int z)
    {
        return biomeData[x, z];
    }

    public string GetData()
    {
        return biomeData.ToString();
    }
}
