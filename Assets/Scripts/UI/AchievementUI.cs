using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementUI : MonoBehaviour
{
    public static Action<AchievementUI> OnButtonClick;

    [SerializeField] private string _achievementName = "Default Achievement";
    [TextArea] [SerializeField] private string _achievementDescription = "Achievement description";
    [SerializeField] private Sprite _achievementSprite;

    private TextMeshProUGUI _achievementButtonText;

    private void Awake()
    {
        _achievementButtonText = transform.Find("AchievementButton").Find("ButtonText").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        setButtonText();
    }

    public void OnAchievementButtonClick()
    {
        OnButtonClick?.Invoke(this);
    }

    public string GetAchievementName()
    {
        return _achievementName;
    }

    public string GetAchievementDescription()
    {
        return _achievementDescription;
    }

    private void setButtonText()
    {
        _achievementButtonText.SetText(_achievementName);
    }
}