using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallController : MonoBehaviour
{
    private Rigidbody _rgb;
    private float _force, _radius;
    
    // Start is called before the first frame update
    void Start()
    {
        _rgb = GetComponent<Rigidbody>();
        
        //StartCoroutine(nameof(RandomBounce));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartBounce()
    {
        DOTween.To(() => _force, x => _force = x, 12500.0f, 3f);
        DOTween.To(() => _radius, x => _radius = x, 7.0f, 3f);
        StartCoroutine(nameof(RandomBounce));
    }

    public async void StopBounce()
    {
        StopCoroutine(nameof(RandomBounce));
        
        await UniTask.WaitForSeconds(1);

        _rgb.isKinematic = true;
    }

    private IEnumerator RandomBounce()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
            _rgb.AddExplosionForce(_force, new Vector3(-0.15f, 5, -0.1f) , _radius);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stopper"))
        {
            _rgb.mass = 1000;
            _rgb.drag = 1000;
            _rgb.angularDrag = 1000;
            _rgb.velocity = Vector3.zero;
        }
    }
}
