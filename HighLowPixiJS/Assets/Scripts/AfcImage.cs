using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class AfcImage : MonoBehaviour
{
    public Image img;
    public string key;
    public Sprite[] languages;
    
    private void OnValidate()
    {
        img = GetComponent<Image>();
    }
}
