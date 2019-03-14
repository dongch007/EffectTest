using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class AsciiArtFx : MonoBehaviour
{
    [Range(4, 64)]
    public int fontPixel = 8;

    public Texture2D asciiTex;
    public int asciiTileX = 4;
    public int asciiTileY = 1;

    [SerializeField] Shader shader;

    private Material _material;
    
    Material material {
        get {
            if (_material == null)
            {
                _material = new Material(shader);
                _material.hideFlags = HideFlags.HideAndDontSave;   
            }
            return _material;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetFloat("_FontPixel", fontPixel);
        material.SetFloat("_TileX", asciiTileX);
        material.SetFloat("_TileY", asciiTileY);
        material.SetTexture("_AsciiTex", asciiTex);
        Graphics.Blit(source, destination, material);
    }
    
    void OnDisable ()
    {
        if (_material) DestroyImmediate(_material);   
    }
}
