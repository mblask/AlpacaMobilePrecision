using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScreenButton : MonoBehaviour
{
    private Button _button;
    private bool _isWindowed;

    private Vector2Int _windowedRes = new Vector2Int(1024, 576);
    private Vector2Int _fullRes = new Vector2Int(1920, 1080);

    private RuntimePlatform _runtimePlatform;

    private void Awake()
    {
        _runtimePlatform = Application.platform;

        if (!_runtimePlatform.Equals(RuntimePlatform.WindowsPlayer) && !_runtimePlatform.Equals(RuntimePlatform.WindowsEditor))
            Destroy(gameObject);

        _button = GetComponent<Button>();
        _isWindowed = Screen.fullScreen;
    }

    private void Start()
    {
        _button.onClick.AddListener(toggleWindowedScreen);
    }

    private void toggleWindowedScreen()
    {
        _isWindowed = !_isWindowed;

        if (!_runtimePlatform.Equals(RuntimePlatform.WindowsPlayer))
            return;

        Vector2Int resolution = _isWindowed ? _windowedRes : _fullRes;
        Screen.SetResolution(resolution.x, resolution.y, _isWindowed);
    }
}
