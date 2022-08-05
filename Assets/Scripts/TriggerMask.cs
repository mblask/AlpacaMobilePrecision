using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMask : MonoBehaviour
{
    private Transform _triggerMaskTransform;
    private SpriteRenderer _triggerMaskRenderer;
    private FadeObject _triggerMaskFade;

    private bool _maskActivated = false;

    private float _timer = 0.0f;
    private float _randomTimingInterval = 1.0f;

    private void Awake()
    {
        _triggerMaskTransform = transform.Find("TriggerMask");
        _triggerMaskRenderer = _triggerMaskTransform.GetComponent<SpriteRenderer>();
        _triggerMaskFade = _triggerMaskTransform.GetComponent<FadeObject>();
    }

    private void Start()
    {
        maskInitialSetup();
    }

    private void Update()
    {
        maskTiming();
    }

    private void maskInitialSetup()
    {
        //scalling the trigger mask to the size of characters
        float characterScaleEqualizationFactor = 1.0f / 0.7f;
        Vector2 maskScale = _triggerMaskTransform.localScale;
        maskScale *= characterScaleEqualizationFactor;
        _triggerMaskTransform.localScale = maskScale;

        //assigning a color to the mask
        _triggerMaskRenderer.color = AlpacaUtils.ChanceFunc(50) ? Color.green : Color.red;
    }

    private void maskTiming()
    {
        _timer += Time.deltaTime;

        if (_timer >= _randomTimingInterval)
        {
            maskActivation();
            _timer = 0.0f;
            float minTimingInterval = 1.0f;
            float maxTimingInterval = 5.0f;

            if (_maskActivated)
                _randomTimingInterval = Random.Range(maxTimingInterval / 2, maxTimingInterval);
            else
                _randomTimingInterval = Random.Range(minTimingInterval / 2, minTimingInterval);
        }
    }

    private void maskActivation()
    {
        if (AlpacaUtils.ChanceFunc(75) && _maskActivated == false)
        {
            _maskActivated = true;
            _triggerMaskRenderer.color = AlpacaUtils.ChanceFunc(50) ? Color.green : Color.red;
        }
        else
            _maskActivated = false;

        _triggerMaskFade.ActivateFade(!_maskActivated);
    }
}
