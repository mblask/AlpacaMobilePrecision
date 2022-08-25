using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMark : MonoBehaviour
{
    private FadeObject _fadeObject;

    [SerializeField] private float _fadeAfter;

    private void Awake()
    {
        _fadeObject = GetComponent<FadeObject>();
    }

    private void Start()
    {
        Invoke(nameof(removeMark), _fadeAfter);
        
        ParticleSystem bulletPS = Instantiate(GameAssets.Instance.BulletMarkPS, transform.position, Quaternion.identity, null).GetComponent<ParticleSystem>();
        bulletPS.Play();
    }

    private void removeMark()
    {
        _fadeObject.ActivateFade(true);
        Destroy(gameObject, _fadeObject.GetFadingTime());
    }
}
