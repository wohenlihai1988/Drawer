using System.IO;
using UnityEditor;
using UnityEngine;

public static class TextureCreator
{

    [MenuItem("Assets/CreateCircleImage")]
    private static void CreateCircleTex()
    {
        var width = 100;
        var center = new Vector2(width / 2, width / 2);
        var tex = new Texture2D(width, width);
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < width; j++)
            {
                var deltax = i - center.x;
                var deltay = j - center.y;
                if(deltax * deltax + deltay * deltay < width * width * 0.25)
                {
                    tex.SetPixel(i, j, Color.white);
                }
                else
                {
                    tex.SetPixel(i, j, Color.clear); 
                }
            }
        }
        var bytes = tex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/Resources/circle.png", bytes);
    }

    [MenuItem("Tools/WhiteToAlpha")]
    public static void WhiteToAlpha()
    {
        var tex = Selection.activeObject as Texture2D;
        if(null == tex)
        {
            return;
        }
        var newTex = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
        var path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        importer.isReadable = true;
        importer.textureFormat = TextureImporterFormat.RGBA32;
        importer.SaveAndReimport();
        var pixels = tex.GetPixels();
        for(int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r > 0.01f)
            {
                pixels[i] = Color.clear;
            }
        }
        newTex.SetPixels(pixels);
        newTex.Apply();
        var bytes = newTex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + path.Replace("Assets", "") + "_new.png", bytes);
    }
}
