using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    // The xSize and zSize are the total bumber of quads on each side of the mesh and not the amount of vertices
    // As such, this should be equal to the desired number of vertices - 1
    public int totalXSize;
    public int totalZSize;

    public WorldGen worldGen;

    // A mesh with 256*256 vertices (65536) cannot be generated as there is a hard limit of 65534 in Unity
    [Range(1, 255)]
    public static int maxMeshSize = 204;

    SubMesh[,] meshes;
    public Material subMeshMaterial;

    public AnimationCurve terrainHeightGraph;

    private void GenerateTerrain()
    {
        if (meshes != null)
        {
            DeleteMeshes();
        }
        meshes = new SubMesh[Mathf.CeilToInt(totalXSize / (float)maxMeshSize), Mathf.CeilToInt(totalZSize / (float)maxMeshSize)];
        for (int i = 0; i < meshes.GetLength(0); i++)
        {
            for (int j = 0; j < meshes.GetLength(1); j++)
            {
                meshes[i, j] = new GameObject("Submesh (" + i + ", " + j + ")", typeof(SubMesh)).GetComponent<SubMesh>();
                meshes[i, j].transform.parent = gameObject.transform;
                meshes[i, j].SetMaterial(subMeshMaterial);
                meshes[i, j].SetBiomeDataSize(GetSubMeshSize(i, totalXSize), GetSubMeshSize(j, totalZSize));
                Debug.Log(GetSubMeshSize(i, totalXSize) + ", " + GetSubMeshSize(j, totalZSize));
                meshes[i, j].position = new Vector2Int(i, j);
            }
        }
        Debug.Log("Mesh grid: " + meshes.GetLength(0) + ", " + meshes.GetLength(1));
        CreateMeshes();
    }

    void OnEnable()
    {
        WorldGen.GeneratedWorld += GenerateTerrain;
    }

    private void OnDisable()
    {
        WorldGen.GeneratedWorld -= GenerateTerrain;
    }

    void CreateMeshes()
    {
        Vector3[] vertices;
        int[] triangles;
        Vector2[] uvs;

        for (int n = 0; n < meshes.GetLength(0); n++)
        {
            for (int m = 0; m < meshes.GetLength(1); m++)
            {
                meshes[n, m].transform.position = new Vector3(n * maxMeshSize, 0, m * maxMeshSize);
                Mesh mesh = meshes[n, m].meshFilter.mesh;

                int xSize = GetSubMeshSize(n, totalXSize);
                int zSize = GetSubMeshSize(m, totalZSize);

                vertices = new Vector3[(xSize + 1) * (zSize + 1)];

                for (int i = 0, z = 0; z <= zSize; z++)
                {
                    for (int x = 0; x <= xSize; i++, x++)
                    {
                        float height = worldGen.GetVertexHeight(z + m * xSize, x + n * zSize);
                        float actualHeight = GetTerrainHeight(height);
                        vertices[i] = new Vector3(x, actualHeight, z);

                        Debug.Log(meshes[n, m].biomeData + " | " + n + ", " + m + " ~ " + x + ", " + z);
                        meshes[n, m].biomeData[x, z].normalizedHeight = height;
                        meshes[n, m].biomeData[x, z].actualHeight = actualHeight;
                        meshes[n, m].biomeData[x, z].temperature = worldGen.GetTerrainTemperature(x, z);
                        meshes[n, m].biomeData[x, z].colour = worldGen.GetTerrainColour(x, z);         
                    }
                }

                triangles = new int[xSize * zSize * 6];

                int vert = 0;
                int tris = 0;
                for (int z = 0; z < zSize; z++)
                {
                    for (int x = 0; x < xSize; x++)
                    {
                        triangles[tris] = vert;
                        triangles[tris + 1] = vert + xSize + 1;
                        triangles[tris + 2] = vert + 1;
                        triangles[tris + 3] = vert + 1;
                        triangles[tris + 4] = vert + xSize + 1;
                        triangles[tris + 5] = vert + xSize + 2;

                        vert++;
                        tris += 6;
                    }
                    vert++;
                }

                uvs = new Vector2[vertices.Length];

                for (int i = 0, z = 0; z <= zSize; z++)
                {
                    for (int x = 0; x <= xSize; i++, x++)
                    {
                        Vector2 uv = new Vector2((x + n * xSize) / (float)totalXSize, (z + m * zSize) / (float)totalZSize);
                        uvs[i] = new Vector2(uv.y, uv.x);
                    }
                }

                mesh.Clear();

                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.uv = uvs;

                mesh.RecalculateNormals();
            }
        }
    }

    int GetSubMeshSize(int meshIndex, int totalMeshSize)
    {
        if (meshIndex < totalMeshSize / maxMeshSize) return maxMeshSize;
        return totalMeshSize % maxMeshSize;
    }

    void DeleteMeshes()
    {
        foreach (SubMesh mesh in meshes)
        {
            Destroy(mesh.gameObject);
        }
    }

    float GetTerrainHeight(float baseHeight)
    {
        return terrainHeightGraph.Evaluate(baseHeight) * 40f;
    }
}