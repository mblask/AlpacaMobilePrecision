using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementUI : MonoBehaviour
{
    public static Action<AchievementUI> OnButtonClick;

    [SerializeField] private bool _isLocked = true;

    [Header("Achievement Properties")]
    [SerializeField] private string _achievementName = "Default Achievement";
    [SerializeField] [TextArea] private string _achievementDescription = "Achievement description";
    [SerializeField] private Sprite _achievementSprite;
    [SerializeField] private AchievementType _achievementType;

    private TextMeshProUGUI _achievementButtonText;
    private IsLockedPanelUI _isLockedPanel;

    private void Awake()
    {
        _achievementButtonText = transform.Find("AchievementButton").Find("ButtonText").GetComponent<TextMeshProUGUI>();
        _isLockedPanel = transform.Find("IsLockedPanelUI").GetComponent<IsLockedPanelUI>();
    }

    private void Start()
    {
        setButtonText();
        _isLockedPanel.ActivatePanel(!_isLocked);
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

    public AchievementType GetAchievementType()
    {
        return _achievementType;
    }

    private void setButtonText()
    {
        _achievementButtonText.SetText(_achievementName);
    }

    public bool IsLocked()
    {
        return _isLocked;
    }

    public void SetLocked(bool value)
    {
        _isLocked = value;
    }
}