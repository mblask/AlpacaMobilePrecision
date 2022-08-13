using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerTouchManager : MonoBehaviour
{
    public Action<Vector2> OnPlayerTouchPosition;

    private static PlayerTouchManager _instance;

    public static PlayerTouchManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private Camera _mainCamera;

    private float _touchTime;

    [Header("For Testing")]
    [SerializeField] private bool _inputActive = true;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        LevelManager.Instance.OnInitializeGame += activateInput;
        GameManager.Instance.OnGameOver += deactivateInput;
        GameManager.Instance.OnGamePaused += onGamePaused;

        _mainCamera = Camera.main;

        _touchTime = 0.0f;
    }

    private void onGamePaused()
    {
        _inputActive = !_inputActive;

        if (_inputActive)
            activateInput();
        else
            deactivateInput();
    }

    private void Update()
    {
        if (!_inputActive)
            return;

        //FOR TESTING, REMOVE LATER
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldClickPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            OnPlayerTouchPosition?.Invoke(worldClickPosition);
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Vector2 worldTouchPosition = _mainCamera.ScreenToWorldPoint(touch.position);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnPlayerTouchPosition?.Invoke(worldTouchPosition);
                    break;

                case TouchPhase.Moved:
                    break;

                case TouchPhase.Stationary:
                    _touchTime += Time.deltaTime;

                    if (_touchTime >= 5.0f)
                        Debug.Log("Stop touching me!");
                    break;

                case TouchPhase.Ended:
                    //Debug.Log("Touch time: " + _touchTime);
                    _touchTime = 0.0f;
                    break;

                case TouchPhase.Canceled:
                    break;

                default:
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnInitializeGame -= activateInput;
        GameManager.Instance.OnGameOver -= deactivateInput;
        GameManager.Instance.OnGamePaused -= onGamePaused;
    }

    private void activateInput()
    {
        _inputActive = true;
    }

    private void deactivateInput()
    {
        _inputActive = false;
    }
}
