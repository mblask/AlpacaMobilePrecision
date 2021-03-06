using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public Action<bool> OnActivateTime;
    public Action<float> OnUpdateTime;
    public Action OnTimeIsOut;

    private static TimeManager _instance;

    public static TimeManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private float _timeDelta = 0.0f;

    [SerializeField] private float _timer;
    private bool _timerOn = false;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        LevelManager.Instance.OnActivateTimer += activateTimer;
    }

    private void Update()
    {
        updateTimer();
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnActivateTimer -= activateTimer;
    }

    private void activateTimer(float timeValue)
    {
        if (timeValue > 0.0f)
        {
            OnActivateTime?.Invoke(true);
            setTimer(timeValue);
            runTimer(true);
        }
        else
            deactivateTimer();
    }

    private void deactivateTimer()
    {
        OnActivateTime?.Invoke(false);
        setTimer(0.0f);
        runTimer(false);
    }

    private void updateTimer()
    {
        if (_timerOn)
        {
            _timeDelta += Time.deltaTime;
            if (_timeDelta > 0.1f)
            {
                _timer -= 0.1f;
                _timeDelta = 0.0f;
                OnUpdateTime?.Invoke(_timer);
            }

            if (_timer <= 0.0f)
            {
                runTimer(false);
                _timer = 0.0f;
                OnTimeIsOut?.Invoke();
                OnUpdateTime?.Invoke(_timer);
            }
        }
    }

    private void setTimer(float timerValue)
    {
        _timer = timerValue;
        OnUpdateTime?.Invoke(_timer);
    }

    private void runTimer(bool value)
    {
        _timerOn = value;
    }
}
