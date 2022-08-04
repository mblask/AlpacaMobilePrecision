using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelTextObject : MonoBehaviour
{
    private TextMeshProUGUI _levelNumberText;

    private void Awake()
    {
        _levelNumberText = transform.Find("LevelNumberText").GetComponent<TextMeshProUGUI>();
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
    }

    private void resetLevelNumberText()
    {
        updateLevelNumberText(1);
    }
}
