using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        ConvertToAlpha((col) =>
        {
            return col.r > 0.99f;
        });
    }

    [MenuItem("Tools/BlackToAlpha")]
    public static void BlackToAlpha()
    {
        ConvertToAlpha((col) =>
        {
            return col.r < 0.01f;
        });
    }

    public static void ConvertToAlpha(Func<Color, bool> filter)
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
            if(filter(pixels[i]))
            {
                pixels[i] = Color.clear;
            }
        }
        newTex.SetPixels(pixels);
        newTex.Apply();
        var bytes = newTex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + path.Replace("Assets", "") + "_new.png", bytes);
    }


    [MenuItem("Tools/TexAreaToTxt")]
    public static void TexAreaToArray()
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
        List<List<int>> list = new List<List<int>>();
        var output = new StringBuilder();
        output.Append("{");
        for (int j = 0; j < tex.height; j++)
        {
            output.Append("{");
            for (int i = 0; i < tex.width; i++)
            {
                var val = tex.GetPixel(i, tex.height - 1 - j).a > 0 ? 1 : 0;
                output.Append(val);
                output.Append(",");
            }
            output.Append("},\n");
        }
        output.Append("}");
        File.WriteAllText(Application.dataPath + path.Replace("Assets", "") + "_array.txt", output.ToString());
    }
}
