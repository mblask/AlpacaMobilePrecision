using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TimeKeeperUI: MonoBehaviour
{
    private Transform _timeNumberTransform;
    private Transform _timeTextTransform;
    private TextMeshProUGUI _textMesh;

    private Animator _animator;
    private string _animatePulseString = "IsPulsing";

    private bool _isEnabled;

    private void Awake()
    {
        _timeTextTransform = transform.Find("TimeText");
        _timeNumberTransform = transform.Find("TimeNumber");
        _textMesh = _timeNumberTransform.GetComponent<TextMeshProUGUI>();

        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        TimeManager.Instance.OnActivateTime += showTimeUI;
        TimeManager.Instance.OnUpdateTime += updateTime;
        GameManager.Instance.OnQuitToMainMenu += hideTimeUI;
    }

    private void OnDestroy()
    {
        TimeManager.Instance.OnActivateTime -= showTimeUI;
        TimeManager.Instance.OnUpdateTime -= updateTime;
        GameManager.Instance.OnQuitToMainMenu -= hideTimeUI;
    }

    private void updateTime(float timeValue)
    {
        string timeText = timeValue.ToString("F1");
        timeText = timeText.Replace(",", ".");
        _textMesh.SetText(timeText);

        if (_isEnabled)
            _animator.SetBool(_animatePulseString, timeValue <= 5.0f && timeValue > 0.0f);
    }

    private void showTimeUI(bool value)
    {
        _isEnabled = value;

        _timeTextTransform.gameObject.SetActive(value);
        _timeNumberTransform.gameObject.SetActive(value);
    }

    private void showTimeUI()
    {
        showTimeUI(true);
    }

    private void hideTimeUI()
    {
        showTimeUI(false);
    }
}