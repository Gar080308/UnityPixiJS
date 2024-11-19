using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScaleAdjust : Editor
{
    [MenuItem("Assets/Scale Adjust")]
    public static void ScaleAdjustment()
    {
        foreach (Transform rootTf in Selection.transforms)
        {
            foreach (Transform tf in rootTf.transform)
            {
                var scale = tf.localScale.x * rootTf.transform.localScale.x;
                tf.localScale = Vector3.one * scale ; 
            }
        }
    }
}
