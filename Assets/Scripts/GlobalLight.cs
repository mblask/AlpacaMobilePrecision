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

    private void OnEnable()
    {
        GameManager.Instance.OnSetLightingIntensity += setGlobalLight;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnSetLightingIntensity -= setGlobalLight;
    }

    private void setGlobalLight(float intensity)
    {
        _globalLight.intensity = intensity;
    }
}
