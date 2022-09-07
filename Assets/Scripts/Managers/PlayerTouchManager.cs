using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerTouchManager : MonoBehaviour
{
    public Action<Vector2> OnPlayerTouchPosition;
    public Action OnDoubleTouch;

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
    private bool _mouseClicked;
    private float _mouseTimer = 0.0f;

    [Header("For Testing")]
    [SerializeField] private bool _inputActive = false;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        LevelManager.Instance.OnInitializeGame += activateInput;
        GameManager.Instance.OnGameOver += deactivateInput;
        GameManager.Instance.OnQuitToMainMenu += deactivateInput;
        GameManager.Instance.OnGamePaused += inputOnGamePaused;

        _mainCamera = Camera.main;

        _touchTime = 0.0f;
    }

    private void Update()
    {
        if (!_inputActive)
            return;

        mouseDoubleClick();

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

            switch (touch.tapCount)
            {
                case 2:
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            //Reload game
                            OnDoubleTouch?.Invoke();
                            break;
                        default:
                            break;
                    }
                    break;

                case 1:

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
        GameManager.Instance.OnQuitToMainMenu -= deactivateInput;
        GameManager.Instance.OnGamePaused -= inputOnGamePaused;
    }

    private void mouseDoubleClick()
    {
        int mouseButton = 0;
        float clickInterval = 0.2f;

        _mouseTimer += Time.deltaTime;

        if (Input.GetMouseButtonDown(mouseButton))
        {
            if (_mouseTimer < clickInterval)
            {
                OnDoubleTouch?.Invoke();
            }
            _mouseTimer = 0.0f;
        }
    }

    private void setInputActive(bool value)
    {
        _inputActive = value;
    }

    private void inputOnGamePaused(bool value)
    {
        setInputActive(!value);
    }

    private void activateInput()
    {
        setInputActive(true);
    }

    private void deactivateInput()
    {
        setInputActive(false);
    }
}
