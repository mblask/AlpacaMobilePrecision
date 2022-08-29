using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundObject : MonoBehaviour
{
    private static BackgroundObject _instance;

    public static BackgroundObject Instance
    {
        get
        {
            return _instance;
        }
    }

    [Header("Settings")]
    [SerializeField] private bool _randomStartingColor = true;
    [SerializeField] private bool _colorsFlow = true;

    private SpriteRenderer _backgroundRenderer;

    private Vector3 _colorChangeSpeed = new Vector3(1.0f, 1.4f, 0.6f);
    private Vector3 _colorChangeDirection = new Vector3(1,1,1);

    private float _colorChangeDamping = 0.1f;

    private void Awake()
    {
        _instance = this;

        _backgroundRenderer = transform.Find("Background").GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Vector2 localScale = AlpacaUtils.GetWorldOrthographicCameraSize();
        _backgroundRenderer.transform.localScale = localScale * 2.0f;

        if (_randomStartingColor)
            randomizeBackgroundColor();
    }

    private void LateUpdate()
    {
        if (_colorsFlow)
            backgroundColorFlow();
    }

    private void backgroundColorFlow()
    {
        float red = _backgroundRenderer.color.r;
        float green = _backgroundRenderer.color.g;
        float blue = _backgroundRenderer.color.b;

        red += _colorChangeDirection.x * Time.deltaTime * _colorChangeDamping * _colorChangeSpeed.x;
        green += _colorChangeDirection.y * Time.deltaTime * _colorChangeDamping * _colorChangeSpeed.y;
        blue += _colorChangeDirection.z * Time.deltaTime * _colorChangeDamping * _colorChangeSpeed.z;

        float minColorAmount = 0.0f;
        float maxColorAmount = 1.0f;

        float minChangeSpeed = 0.1f;
        float maxChangeSpeed = 2.0f;

        if (red >= maxColorAmount || red <= minColorAmount)
        {
            _colorChangeDirection.x *= -1;
            _colorChangeSpeed.x = Random.Range(minChangeSpeed, maxChangeSpeed);
        }

        if (green >= maxColorAmount || green <= minColorAmount)
        {
            _colorChangeDirection.y *= -1;
            _colorChangeSpeed.y = Random.Range(minChangeSpeed, maxChangeSpeed);
        }

        if (blue >= maxColorAmount || blue <= minColorAmount)
        {
            _colorChangeDirection.z *= -1;
            _colorChangeSpeed.z = Random.Range(minChangeSpeed, maxChangeSpeed);
        }

        Color newColor = new Color(red, green, blue);
        _backgroundRenderer.color = newColor;
    }

    private void randomizeBackgroundColor()
    {
        Vector3 newColor = new Vector3();

        newColor.x = Random.Range(0.0f, 1.0f);
        newColor.y = Random.Range(0.0f, 1.0f);
        newColor.z = Random.Range(0.0f, 1.0f);

        _backgroundRenderer.color = new Color(newColor.x, newColor.y, newColor.z);
    }

    private void activateRandomStartColor(bool value)
    {
        _randomStartingColor = value;
    }

    private void activateColorFlow(bool value)
    {
        _colorsFlow = value;
    }

    public Color GetBackgroundColor()
    {
        return _backgroundRenderer.color;
    }

    public static Color GrabBackgroundColor()
    {
        return _instance.GetBackgroundColor();
    }
}
