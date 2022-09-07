using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }
}
