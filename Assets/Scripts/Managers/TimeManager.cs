using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public Action<bool> OnActivateTime;
    public Action<float> OnUpdateTime;
    public Action OnLastFiveSeconds;
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
    private bool _lastFiveReached = false;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        LevelManager.Instance.OnActivateTimer += activateTimer;
        GameManager.Instance.OnGameOver += StopTimer;
        GameManager.Instance.OnQuitToMainMenu += StopTimer;
    }

    private void Update()
    {
        updateTimer();
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnActivateTimer -= activateTimer;
        GameManager.Instance.OnGameOver += StopTimer;
        GameManager.Instance.OnQuitToMainMenu += StopTimer;
    }

    private void activateTimer(float timeValue)
    {
        setTimer(0.0f);

        if (timeValue > 0.0f)
        {
            OnActivateTime?.Invoke(true);
            setTimer(timeValue);
            RunTimer();
        }
        else
            deactivateTimer();
    }

    private void deactivateTimer()
    {
        OnActivateTime?.Invoke(false);
        StopTimer();
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

            if (_timer <= 5.0f && !_lastFiveReached)
            {
                _lastFiveReached = true;
                Debug.Log("Last five seconds!");
                OnLastFiveSeconds?.Invoke();
            }

            if (_timer <= 0.0f)
            {
                StopTimer();
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

    public static float GrabTimerValue()
    {
        return _instance.GetTimerValue();
    }

    public float GetTimerValue()
    {
        if (_timerOn)
            return _timer;
        else
            return -1.0f;
    }

    public void StopTimer()
    {
        _timerOn = false;
    }

    public void RunTimer()
    {
        _timerOn = true;
    }
}
