using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExportImgAssets : Editor
{
    [MenuItem("Assets/Export Img Assets")]
    public static void ExportImgAssetsFunc()
    {
        var s = "";
        foreach (var obj in Selection.objects)
        {
           s += $"{obj.name}:assetPath+'{obj.name}.png',";
        }
        string code = "Assets.addBundle('imgAssets', {"+s+"})";
        Debug.Log(code);
    }
}
