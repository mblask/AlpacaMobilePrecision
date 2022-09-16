using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

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
        //assigning random Character sprite to the mask
        _triggerMaskRenderer.sprite = GameAssets.Instance.CharacterSprites.GetRandomElement();

        //assigning a color to the mask
        _triggerMaskRenderer.color = Utilities.ChanceFunc(50) ? Color.green : Color.red;
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
        if (Utilities.ChanceFunc(75) && _maskActivated == false)
        {
            _maskActivated = true;
            _triggerMaskRenderer.color = Utilities.ChanceFunc(50) ? Color.green : Color.red;
        }
        else
            _maskActivated = false;

        _triggerMaskFade.ActivateFade(!_maskActivated);
    }
}
