using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerToucher : MonoBehaviour
{
    public Action<Vector3> OnPlayerTouchPosition;

    private static PlayerToucher _instance;

    public static PlayerToucher Instance
    {
        get
        {
            return _instance;
        }
    }

    private Camera _mainCamera;

    private float _touchTime;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _mainCamera = Camera.main;

        _touchTime = 0.0f;
    }

    private void Update()
    {
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
}
