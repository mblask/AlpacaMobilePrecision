using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class WantedCharacterUI : MonoBehaviour
{
    public static Action<TooltipParameters> OnButtonClick;

    private bool _isLocked = true;

    private WantedCharacter _wantedCharacter;

    private TextMeshProUGUI _buttonText;
    private IsLockedPanelUI _isLockedPanelUI;

    private void Awake()
    {
        _buttonText = transform.Find("WantedCharacterButton").Find("ButtonText").GetComponent<TextMeshProUGUI>();
        _isLockedPanelUI = transform.Find("IsLockedPanelUI").GetComponent<IsLockedPanelUI>();
    }

    public void SetupUI(WantedCharacter wantedCharacter)
    {
        _wantedCharacter = wantedCharacter;
        setButtonText(wantedCharacter.WantedName);
    }

    private void setButtonText(string text)
    {
        _buttonText.SetText(text);
    }

    public void OnWantedCharacterButtonClick()
    {
        string description = "A level " + _wantedCharacter.WantedLevel + " wanted character.";
        OnButtonClick?.Invoke(new TooltipParameters { Position = transform.position, Title = _wantedCharacter.WantedName, Description = description });
    }

    public bool IsLocked()
    {
        return _isLocked;
    }

    public void SetLocked(bool value)
    {
        _isLocked = value;
        _isLockedPanelUI.ActivatePanel(!value);
    }
}
