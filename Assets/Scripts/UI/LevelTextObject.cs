using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelTextObject : MonoBehaviour
{
    private TextMeshProUGUI _levelNumberText;
    private Animator _animator;
    private string _levelChangedTrigger = "LevelChanged";

    private void Awake()
    {
        _levelNumberText = transform.Find("LevelNumberText").GetComponent<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        LevelManager.Instance.OnLoadLevel += updateLevelNumberText;
        LevelManager.Instance.OnGameReload += resetLevelNumberText;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnLoadLevel -= updateLevelNumberText;
        LevelManager.Instance.OnGameReload += resetLevelNumberText;
    }

    private void updateLevelNumberText(int levelNumber)
    {
        _levelNumberText.SetText(levelNumber.ToString());
        _animator.SetTrigger(_levelChangedTrigger);
    }

    private void resetLevelNumberText()
    {
        updateLevelNumberText(1);
    }
}
