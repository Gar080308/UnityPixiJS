using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

public class CaptureAnimation : MonoBehaviour
{
    private BallController[] _balls;

    private List<List<BallPOS>> _pos, _rot;

    private bool _started, _ended;
    
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        _balls = FindObjectsOfType<BallController>();
        _pos = new List<List<BallPOS>>();
        _rot = new List<List<BallPOS>>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_started && !_ended)
        {
            var index = 0;
            foreach (var b in _balls)
            {
                _pos[index].Add(new BallPOS()
                {
                    x = b.transform.position.x,
                    y = b.transform.position.y,
                    z = b.transform.position.z
                });
                
                _rot[index].Add(new BallPOS()
                {
                    x = b.transform.eulerAngles.x,
                    y = b.transform.eulerAngles.y,
                    z = b.transform.eulerAngles.z
                });
                index++;
            }
        }
    }

    [Button]
    public void Capture()
    {
        _started = true;
        foreach (var b in _balls)
        {
            _pos.Add(new List<BallPOS>());
            _rot.Add(new List<BallPOS>());
            b.StartBounce();
        }
    }
    
    [Button]
    public void Result()
    {
        _started = true;
        _balls[0].transform.DOMoveX(-3.2f, 0.1f);
        foreach (var b in _balls)
        {
            _pos.Add(new List<BallPOS>());
            _rot.Add(new List<BallPOS>());
            b.StartBounce();
        }
    }

    [Button]
    public async void End()
    {
        foreach (var b in _balls)
        {
            b.StopBounce();
        }

        await UniTask.WaitForSeconds(1);
        
        _ended = true;

        File.WriteAllText(Application.dataPath + "/pathData.txt", JsonConvert.SerializeObject(_pos));
        File.WriteAllText(Application.dataPath + "/rotData.txt", JsonConvert.SerializeObject(_rot));
        File.WriteAllText(Application.dataPath + "/resultData.txt", JsonConvert.SerializeObject(_pos));
    }
}

[Serializable]
public class BallPOS
{
    public float x;
    public float y;
    public float z;
}
