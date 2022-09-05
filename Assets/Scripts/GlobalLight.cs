using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GlobalLight : MonoBehaviour
{
    private Light2D _globalLight;

    private void Awake()
    {
        _globalLight = GetComponent<Light2D>();
    }

    private void Start()
    {
        GameManager.Instance.OnSetDifficulty += setGlobalLight;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnSetDifficulty -= setGlobalLight;
    }

    private void setGlobalLight(float intensity)
    {
        _globalLight.intensity = intensity;
    }
}
