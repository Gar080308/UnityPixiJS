using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using TMPro;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ExportSceneToJSON : Editor
{
    private  const string Prefix3D = "t3dp";
    
    private static Dictionary<string, List<string>> _parentDict;
    private static Dictionary<string, string> _childDict;
    private static Dictionary<string, RectTransform> _objectDict;
    private static Transform _canvas, _cloneCanvas;
    private static string _c;

    [MenuItem("Assets/Set Anchor")]
    public static void SetAnchor()
    {
        foreach (Transform transform in Selection.transforms[0].transform)
        {
            var obj = (RectTransform)transform;
            var localPos = obj.localPosition;
            obj.anchorMin = new Vector2(0, 1);
            obj.anchorMax = new Vector2(0, 1);
            obj.localPosition = localPos;
        }
    }
    
    [MenuItem("Assets/Export Scene to JSON")]
    public static async void Export()
    {
        var T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        var getSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Vector2 res = (Vector2) getSizeOfMainGameView.Invoke(null,null);
        
        _canvas = null;
        _parentDict = new Dictionary<string, List<string>>();
        _childDict = new Dictionary<string, string>();
        _objectDict = new Dictionary<string, RectTransform>();
        _cloneCanvas = GameObject.Find("CloneCanvas").transform;
        
        string s = "";
        string f = "";
        string a = "";
        string l = "const lImages = []; const lTexts = [];";
        _c = "";
        
        var folder = EditorUtility.OpenFolderPanel("Export JSON into what folder?", "", "");

        foreach (Transform obj in Selection.transforms)
        {
            if (_canvas == null)
                _canvas = obj;
        }
        
        GetParentData(null, Selection.transforms[0]);
        await GetObjectData(Selection.transforms[0].GetComponent<RectTransform>());

        // Debug.Log(JsonConvert.SerializeObject(_parentDict));
        // Debug.Log(JsonConvert.SerializeObject(_objectDict,new JsonSerializerSettings()
        // {
        //     ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        // }));

        foreach (var obj in _objectDict)
        {
            var isContainer = true;
            
            var img = obj.Value.GetComponent<Image>();
            var lImage = obj.Value.GetComponent<AfcImage>();
            var lText = obj.Value.GetComponent<AfcText>();
            var threeD = obj.Key.Contains(Prefix3D) ? "2d" : "";
            
            if (img != null && img.sprite != null && !obj.Value.GetComponent<TMP_InputField>())
            {
                isContainer = false;
                var pos = obj.Value.anchoredPosition;
                var scale = obj.Value.localScale;
                var pivot = obj.Value.pivot;
                
                if (img.type == Image.Type.Sliced)
                { 
                    var sizeDelta = obj.Value.sizeDelta;
                   s += $"const {obj.Key} = new NineSlicePlane(sheet.{img.sprite.name}, {img.sprite.border.x} ,{img.sprite.border.y} ,{img.sprite.border.z} ,{img.sprite.border.w});";
                   s += $"{obj.Key}.width = {sizeDelta.x};";
                   s += $"{obj.Key}.height = {sizeDelta.y};";
                   s += $"{obj.Key}.x = {pos.x - sizeDelta.x/2};";
                   s += $"{obj.Key}.y = {-pos.y - sizeDelta.y/2};";
                }
                else
                {
                    if (!obj.Value.GetComponent<Animator>())
                        s += $"const {obj.Key} = new Sprite{threeD}(sheet.{img.sprite.name});";
                    else
                    {
                        //s += $"const {obj.Key} = new AnimatedSprite(sheet.animations['TakeThisOut']);";
                        Debug.Log("Manually add this : ");
                        Debug.Log($"const {obj.Key} = new AnimatedSprite(sheet.animations['']);");
                    }
                        
                    s += $"{obj.Key}.anchor.x = {pivot.x};";
                    s += $"{obj.Key}.anchor.y = {1-pivot.y};";
                    s += $"{obj.Key}.x = {pos.x};";
                    s += $"{obj.Key}.y = {-pos.y};";
                }
                
                // s += $"{obj.Key}.anchor.x = 0.5;";
                // s += $"{obj.Key}.anchor.y = 0.5;";

                if (lImage != null)
                {
                    l += $"lImages.push({obj.Key});";
                    l += $"{obj.Key}.loc = [\"{lImage.languages[0].name}.png\",\"{lImage.languages[1].name}.png\",\"{lImage.languages[2].name}.png\"];";
                }
                
                s += $"{obj.Key}.dpx = {pos.x};";
                s += $"{obj.Key}.dpy = {-pos.y};";
                s += $"{obj.Key}.scale.set({scale.x},{scale.y});";
                s += $"{obj.Key}.dsx = {scale.x};";
                s += $"{obj.Key}.dsy = {scale.y};";
                s += $"{obj.Key}.tint = '#{ColorUtility.ToHtmlStringRGBA(img.color)}';";
                s += $"{obj.Key}.alpha = {img.color.a};";
                s += $"{obj.Key}.rotation = {-obj.Value.localEulerAngles.z / 180 * 3.14f};";
            }

            var btn = obj.Value.GetComponent<Button>();
            
            if (btn != null)
            {
                isContainer = false;
                s += $"{obj.Key}.eventMode = \"static\";";
                //s += obj.Key + ".on(\"click\", (event)=>{console.log(\"Clicked "+ obj.Key+"\");});";
                s += obj.Key + $".on(\"pointerdown\",{obj.Key}_Click);";
                s += obj.Key + ".disabled = false;";
                f += "function "+obj.Key+"_Click(){ console.log('"+obj.Key+" clicked');  }";
            }
            
            var text = obj.Value.GetComponent<TextMeshProUGUI>();

            if (text != null)
            {
                isContainer = false;
                var pos = obj.Value.anchoredPosition;
                var scale = obj.Value.localScale;
                var pivot = obj.Value.pivot;
                // if(text.overrideColorTags)
                //     s += $"const {obj.Key} = new BitmapText('{text.text}', new TextStyle(" + "{  fontName: 'Anton', fontSize: "+text.fontSize+" }));";
                if(text.overrideColorTags)
                    s += $"const {obj.Key} = new TaggedText('[Replace manually]', new TextStyle({{ fill : ['#{ColorUtility.ToHtmlStringRGB( text.color )}'], wordWrap: true, wordWrapWidth: {text.rectTransform.sizeDelta.x},lineHeight : {(int)(text.fontSize * 1.5f)} ,fontFamily: 'OpenSans', fontSize: {text.fontSize} }}));";
                else
                {
                    s += $"const {obj.Key} = new Text{threeD}('{text.text}', new TextStyle({{ fill : ['#{ColorUtility.ToHtmlStringRGB( text.color )}'], wordWrap: true, wordWrapWidth: {text.rectTransform.sizeDelta.x},lineHeight : {(int)(text.fontSize * 1.5f)} ,fontFamily: 'OpenSans', fontSize: {text.fontSize} }}));";
                }
                    
                if (lText != null)
                {
                    l += $"lTexts.push({obj.Key});";
                    l += $"{obj.Key}.loc = [\"{lText.languages[0]}\",\"{lText.languages[1]}\",\"{lText.languages[2]}\"];";
                }
                
                s += $"{obj.Key}.x = {pos.x};";
                s += $"{obj.Key}.y = {-pos.y};";
                if(text.horizontalAlignment == HorizontalAlignmentOptions.Left)
                    s += $"{obj.Key}.anchor.x = 0;";
                else if(text.horizontalAlignment == HorizontalAlignmentOptions.Center)
                    s += $"{obj.Key}.anchor.x = 0.5;";
                else if(text.horizontalAlignment == HorizontalAlignmentOptions.Right)
                    s += $"{obj.Key}.anchor.x = 1;";
                s += $"{obj.Key}.anchor.y = {1-pivot.y};";
                s += $"{obj.Key}.scale.set({scale.x},{scale.y});";
            }
            
            var spine = obj.Value.GetComponent<SkeletonGraphic>();

            if (spine != null)
            {
                isContainer = false;
                var pos = obj.Value.anchoredPosition;
                var scale = obj.Value.localScale;
                a += $"const {obj.Key}_SR = await Assets.load(\"assets/{spine.skeletonDataAsset.skeletonJSON.name}.json\");";
                a += $"const {obj.Key} = new Spine({obj.Key}_SR.spineData);";
                a += $"{obj.Key}.x = {pos.x};";
                a += $"{obj.Key}.y = {-pos.y};";
                // a += $"{obj.Key}.anchor.x = 0.5;";
                // a += $"{obj.Key}.anchor.y = 0.5;";
                a += $"{obj.Key}.scale.set({scale.x},{scale.y});";
            }
            
            var ipf = obj.Value.GetComponent<TMP_InputField>();
            
            if (ipf != null)
            {
                isContainer = false;
                var pos = obj.Value.anchoredPosition;
                var scale = obj.Value.localScale;
                var sizeDelta = obj.Value.sizeDelta;
                s += $"const {obj.Key} = new TextInput({{ input: {{ fontSize: '64px', padding: '28px', width: '{sizeDelta.x}px', color: '#26272E' }}, box: {{ default: {{ fill: 15264243, rounded: 72, stroke: {{ color: 13356768, width: 3 }} }}, focused: {{ fill: 14803950, rounded: 72, stroke: {{ color: 11251654, width: 3 }} }}, disabled: {{ fill: 14408667, rounded: 72 }} }} }}); {obj.Key}.placeholder = '...';";
                s += $"{obj.Key}.width = {sizeDelta.x};";
                s += $"{obj.Key}.height = {sizeDelta.y};";
                s += $"{obj.Key}.x = {pos.x - sizeDelta.x/2};";
                s += $"{obj.Key}.y = {-pos.y - sizeDelta.y/2};";
                s += $"{obj.Key}.scale.set({scale.x},{scale.y});";
                
                Debug.Log("Make sure to delete all child of TMP_InputField");
            }

            if(isContainer)
            {
                var pos = obj.Value.anchoredPosition;
                s += $"const {obj.Key} = new Container{threeD}();";
                if (!obj.Value.gameObject.activeInHierarchy)
                {
                    s += $"{obj.Key}.visible = false;";
                }

                if (obj.Value.GetComponent<ScrollRect>())
                {
                    s += $"{obj.Key}.x = {pos.x};";
                    s += $"{obj.Key}.y = {pos.y};";
                }
                
                var mask = obj.Value.GetComponent<Mask>();
                if (mask != null)
                {
                    var size = obj.Value.sizeDelta;
                    s += $"{obj.Key}.mask = new Graphics().beginFill(0xffffff).drawRect({(res.x - size.x) / 2},{size.y},{size.x},{size.y}).endFill();";
                }
            }
            
            if(_childDict[obj.Key] == "Canvas")
                s += $"app.stage.addChild({obj.Key});";
        }

        foreach (var obj in _parentDict)
        {
            if(obj.Key == "Canvas" /*|| !string.IsNullOrEmpty(_objectDict[obj.Key].sprite)*/) continue;
            foreach (var child in obj.Value)
            {
                s += $"{obj.Key}.addChild({child});";
            }
        }

        s += _c;
        
        Debug.Log(_c);
        
        await File.WriteAllTextAsync(Application.dataPath + "/ExportedData.js", s);
        await File.WriteAllTextAsync(Application.dataPath + "/ExportedFunction.js", f);
        await File.WriteAllTextAsync(Application.dataPath + "/ExportedAnimation.js", a);
        await File.WriteAllTextAsync(Application.dataPath + "/ExportedLocalization.js", l);
        
        Debug.Log("Exported!");
    }

    private static void GetParentData(string parent, Transform obj)
    {
        if (parent != null)
        {
            if (!_parentDict.ContainsKey(parent))
                _parentDict.Add(parent, new List<string>());

            _parentDict[parent].Add(obj.name);
        }

        foreach (Transform c in obj.transform)
        {
            GetParentData(obj.name, c);
        }
    }

    private static async Task GetObjectData(RectTransform obj)
    {
        if (obj.transform != _canvas)
        {
            var parent = obj.parent;

            _childDict[obj.name] = parent.name;

            try
            {
                obj.SetParent(_cloneCanvas);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            
            await UniTask.Delay(10);
           
            _objectDict.Add(obj.name, obj);
        }
        
        if (obj.name.Contains("Content_"))
        {
            var subFix = obj.name.Split("_")[1];
            foreach (Transform c in obj.transform)
            {
                if (!c.name.Contains("_"))
                {
                    c.name = c.name + "_" + subFix;
                }

                _c += $"{obj.name}.{c.name.Split("_")[0]} = {c.name};";
            }
        }

        if (obj.GetComponent<SkeletonGraphic>() != null) return;
        
        while (obj.childCount > 0)
        {
            try
            {
                await GetObjectData(obj.GetChild(0).GetComponent<RectTransform>());
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            
        }
    }
}
