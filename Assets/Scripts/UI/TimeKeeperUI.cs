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

    private void Awake()
    {
        _timeTextTransform = transform.Find("TimeText");
        _timeNumberTransform = transform.Find("TimeNumber");
        _textMesh = _timeNumberTransform.GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        showTimeUI(false);

        TimeManager.Instance.OnActivateTime += showTimeUI;
        TimeManager.Instance.OnUpdateTime += updateTime;
        LevelManager.Instance.OnGameReload += levelManager_onGameReload;
    }

    private void OnDestroy()
    {
        TimeManager.Instance.OnActivateTime -= showTimeUI;
        TimeManager.Instance.OnUpdateTime -= updateTime;
        LevelManager.Instance.OnGameReload -= levelManager_onGameReload;
    }

    private void levelManager_onGameReload()
    {
        showTimeUI(false);
    }

    private void updateTime(float timeValue)
    {
        string timeNumber = timeValue.ToString("F1");
        timeNumber = timeNumber.Replace(",", ".");
        _textMesh.SetText(timeNumber);
    }

    private void showTimeUI(bool value)
    {
        _timeTextTransform.gameObject.SetActive(value);
        _timeNumberTransform.gameObject.SetActive(value);
    }
}