using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RenameContent : Editor
{
    private  const string Prefix3D = "t3dp";
    
    [MenuItem("Assets/Rename Content")]
    public static void Rename()
    {
        foreach (Transform obj in Selection.transforms)
        {
            if (obj.name.Contains("Content_"))
            {
                var subFix = obj.name.Split("_")[1];
                foreach (Transform tf in obj.transform)
                {
                    tf.name += "_" + subFix;
                }
            }
        }
    }
    
    [MenuItem("Assets/Remove 3D Prefix")]
    public static void Remove3DPrefix()
    {
        void Action(Transform tf)
        {
            if(tf.name.Contains(Prefix3D)) tf.name = tf.name.Replace(Prefix3D, "");
            foreach (Transform obj in tf.transform)
            {
                if (obj.name.Contains(Prefix3D)) obj.name = obj.name.Replace(Prefix3D, "");
                if (obj.childCount > 0) Action(obj);
            }
        }

        Action(Selection.transforms[0]);
    }
    
    [MenuItem("Assets/Add 3D Prefix")]
    public static void Add3DPrefix()
    {
        void Action(Transform tf)
        {
            if (!tf.name.Contains(Prefix3D)) tf.name = Prefix3D + tf.name;
            foreach (Transform obj in tf.transform)
            {
                if (!obj.name.Contains(Prefix3D)) obj.name = Prefix3D + obj.name;
                if (obj.childCount > 0) Action(obj);
            }
        }

        Action(Selection.transforms[0]);
    }
}
