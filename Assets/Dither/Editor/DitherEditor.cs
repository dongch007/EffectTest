using UnityEngine;
using UnityEditor;
using System.IO;

public class DitherEditor : EditorWindow
{
    [MenuItem("EffectTest/Dither")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DitherEditor));
    }

    //https://en.wikipedia.org/wiki/Ordered_dithering
    int[,] bayer2x2 = new int[2,2]{{0,2},{3,1}};
    //int[][] bayer9 = { 0, 7, 3,
    //                 6, 5, 2,
    //                 4, 1, 8};

    int[,] GetBayer(int dim)
    {
        if (dim == 2)
            return bayer2x2;

        int[,] bayer = new int[dim,dim];

        int preDim = dim / 2;
        int[,] preBayer = GetBayer(preDim);

        for(int i = 0; i < preDim; i++)
        {
            for (int j = 0; j < preDim; j++)
            {
                bayer[i, j] = preBayer[i, j] * 4;
            }
        }

        for (int i = 0; i < preDim; i++)
        {
            for (int j = 0; j < preDim; j++)
            {
                bayer[i, j+ preDim] = preBayer[i, j] * 4 + 2;
            }
        }

        for (int i = 0; i < preDim; i++)
        {
            for (int j = 0; j < preDim; j++)
            {
                bayer[i+ preDim, j] = preBayer[i, j] * 4 + 3;
            }
        }

        for (int i = 0; i < preDim; i++)
        {
            for (int j = 0; j < preDim; j++)
            {
                bayer[i+ preDim, j+ preDim] = preBayer[i, j] * 4 + 1;
            }
        }


        return bayer;
    }

    string text = "16";
    void OnGUI()
    {
        text = GUILayout.TextField(text);

        if (GUILayout.Button("Generate Texture"))
        {
            int size = 16;
            if (int.TryParse(text, out size) == false)
            {
                EditorUtility.DisplayDialog("error", "Should be a number", "OK");
                return;
            }

            if(Mathf.IsPowerOfTwo(size) == false)
            {
                EditorUtility.DisplayDialog("error", "size is not power of two", "OK");
                return;
            }

            int[,] bayer = this.GetBayer(size);


            Texture2D texture = new Texture2D(size, size);

            Color32[] colors = new Color32[size * size];
            for (int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    byte c = (byte)(((float)(bayer[i, j]+0.5f) / (size * size)) * 255);

                    Color32 color = new Color32();
                    color.r = c;
                    color.g = c;
                    color.b = c;
                    color.a = 255;
                    colors[i * size + j] = color;
                }
            }
            texture.SetPixels32(colors);
            string path = Application.dataPath + "/Dither/dither" + size + ".png";
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);

            path = "Assets/Dither/dither" + size + ".png";
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}