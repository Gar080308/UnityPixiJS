using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class AfcText : MonoBehaviour
{
    public TextMeshProUGUI tmpUGUI;
    public string key;
    public string[] languages;
    
    private void OnValidate()
    {
        tmpUGUI = GetComponent<TextMeshProUGUI>();
    }
}
