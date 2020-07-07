using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMap : MonoBehaviour
{
    public Renderer mapRenderer;

    public void DrawTexture(Texture2D texture)
    {
        mapRenderer.sharedMaterial.mainTexture = texture;
        mapRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }
}
