using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshBuilder : MonoBehaviour
{
    Mesh mesh;

    Vector3[] verts;
    int[] tris;

    const int meshWidth = 8;
    const int meshHeight = 8;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        StartCoroutine(CreateMesh());
        UpdateMesh();
    }

    IEnumerator CreateMesh()
    {
        verts = new Vector3[(meshWidth + 1) * (meshHeight + 1)];

        for (int i = 0, z = 0; z <= meshWidth; z++)
        {
            for (int x = 0; x <= meshHeight; x++)
            {
                verts[i] = new Vector3(x, 0, z);
                i++;
            }
        }

        tris = new int[meshWidth * meshHeight * 6];

        int v = 0;
        int t = 0;
        for (int z = 0; z < meshHeight; z++)
        {
            for (int x = 0; x < meshWidth; x++)
            {
                tris[t] = v;
                tris[t + 1] = v + x + 1;
                tris[t + 2] = v + 1;
                tris[t + 3] = v + 1;
                tris[t + 4] = v + x + 1;
                tris[t + 5] = v + x + 2;

                v++;
                t += 6;
                yield return null;
            }
            v++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = verts;
        mesh.triangles = tris;

        mesh.RecalculateNormals();
    }
}
