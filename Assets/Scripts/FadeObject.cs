using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FadeOnOff
{
    Fade,
    Appear,
}

public class FadeObject : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private float _initialFadingSpeed = 1.5f;
    private float _fadingSpeed = 1.5f;
    private bool _invisible = false;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sharedMaterial = new Material(_spriteRenderer.sharedMaterial);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        _spriteRenderer.sharedMaterial.SetFloat("_DissolveIntensity", 1.0f);
    }

    public bool IsInvisible()
    {
        return _invisible;
    }

    public void ActivateFade(bool fadeOnOff)
    {
        StartCoroutine("Fade", fadeOnOff);
    }

    public IEnumerator Fade(bool value)
    {
        _invisible = value;
        float elapsedTime = 0.0f;

        float fadeDuration = 1 / _fadingSpeed;
        string dissolveIntensity = "_DissolveIntensity";
        float minDissolveIntensity = -0.05f;
        float maxDissolveIntensity = 1.0f;

        float dissolveMagnitude = _spriteRenderer.sharedMaterial.GetFloat(dissolveIntensity);

        while (elapsedTime < fadeDuration)
        {
            if (value)
            {
                dissolveMagnitude -= _fadingSpeed * Time.deltaTime;

                if (dissolveMagnitude > minDissolveIntensity)
                    _spriteRenderer.sharedMaterial.SetFloat(dissolveIntensity, dissolveMagnitude);
            }
            else
            {
                dissolveMagnitude += _fadingSpeed * Time.deltaTime;

                if (dissolveMagnitude <= maxDissolveIntensity)
                    _spriteRenderer.sharedMaterial.SetFloat(dissolveIntensity, dissolveMagnitude);
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }

    public void SetFadingSpeed(float value)
    {
        _fadingSpeed = value;
    }

    public void ResetFadingSpeed()
    {
        _fadingSpeed = _initialFadingSpeed;
    }

    public float GetFadingTime()
    {
        return 1 / _fadingSpeed;
    }
}
