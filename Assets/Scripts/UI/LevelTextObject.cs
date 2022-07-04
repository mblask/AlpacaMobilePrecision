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
    }

    private void OnDestroy()
    {

        LevelManager.Instance.OnLoadLevel -= updateLevelNumberText;
    }

    private void updateLevelNumberText(int levelNumber)
    {
        _levelNumberText.SetText(levelNumber.ToString());
    }
}
