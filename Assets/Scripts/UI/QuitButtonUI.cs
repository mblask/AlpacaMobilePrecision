using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuitButtonUI : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(() => {
            SaveManager.SaveProgress();
        });
    }

    private void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            _button.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().SetText("WebGL Ver.");
            _button.interactable = false;
        }
    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }
}
