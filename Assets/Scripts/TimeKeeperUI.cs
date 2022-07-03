using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TimeKeeperUI: MonoBehaviour
{
    private Transform _timeTransform;
    private TextMeshProUGUI _textMesh;

    private void Awake()
    {
        _timeTransform = transform.Find("Time");
        _textMesh = _timeTransform.GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        showTimeUI(false);

        TimeManager.Instance.OnActivateTime += showTimeUI;
        TimeManager.Instance.OnUpdateTime += updateTime;
    }

    private void OnDestroy()
    {
        TimeManager.Instance.OnActivateTime -= showTimeUI;
        TimeManager.Instance.OnUpdateTime -= updateTime;
    }

    private void updateTime(float timeValue)
    {
        _textMesh.SetText(timeValue.ToString("F1"));
    }

    private void showTimeUI(bool value)
    {
        _timeTransform.gameObject.SetActive(value);
    }
}