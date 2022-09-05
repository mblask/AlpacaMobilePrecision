using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AccuracyKeeperUI : MonoBehaviour
{
    private Transform _accuracyTextTransform;
    private Transform _accuracyNumberTransform;
    private TextMeshProUGUI _accuracyNumber;

    private float _accuracyNeeded = 0;

    private Animator _animator;
    private string _accuracyChangedTrigger = "AccuracyChanged";

    private void Awake()
    {
        _accuracyNumberTransform = transform.Find("AccuracyNumber");
        _accuracyTextTransform = transform.Find("AccuracyText");
        _accuracyNumber = _accuracyNumberTransform.GetComponent<TextMeshProUGUI>();

        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        showAccuracy(false);

        LevelManager.Instance.OnGameReload += resetAccuracyNeeded;
        LevelManager.Instance.OnActivateAccuracy += levelManager_onActivateAccuracy;
        HitManager.Instance.OnSendPlayerAccuracy += updateAccuracy;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnGameReload -= resetAccuracyNeeded;
        LevelManager.Instance.OnActivateAccuracy -= levelManager_onActivateAccuracy;
        HitManager.Instance.OnSendPlayerAccuracy -= updateAccuracy;
    }

    private void levelManager_onActivateAccuracy(float accuracy)
    {
        showAccuracy(accuracy > 0.0f);
        setAccuracyNeeded(accuracy);
        updateAccuracy(0.0f);
    }

    private void updateAccuracy(float value)
    {
        float accuracyValue = value * 100.0f;
        _accuracyNumber.SetText(accuracyValue.ToString("F0") + " / " + _accuracyNeeded.ToString("F0"));
        _animator.SetTrigger(_accuracyChangedTrigger);
    }

    private void setAccuracyNeeded(float value)
    {
        float accuracyNeededValue = value * 100.0f;
        _accuracyNeeded = accuracyNeededValue;
    }

    private void resetAccuracyNeeded()
    {
        setAccuracyNeeded(0);
    }

    private void showAccuracy(bool value)
    {
        _accuracyNumberTransform.gameObject.SetActive(value);
        _accuracyTextTransform.gameObject.SetActive(value);
    }
}
