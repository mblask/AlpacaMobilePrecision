using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class DifficultyUI : MonoBehaviour
{
    public static Action<TooltipParameters> OnButtonClick;

    private Button _button;
    private TMP_Dropdown _dropdown;
    public static Action<Difficulty> OnDifficultyChanged;

    private void Awake()
    {
        _button = transform.Find("DifficultyText").GetComponent<Button>();
        _dropdown = transform.Find("DifficultyDropdown").GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        _button.onClick.AddListener(onButtonClick);
        _dropdown.onValueChanged.AddListener(difficultyChanged);
    }

    private void onButtonClick()
    {
        Vector2 position = transform.position;
        string title = "Tip";
        string description = "It's just the lighting intensity, chill dude...";

        TooltipParameters parameters = new TooltipParameters { Position = position, Title = title, Description = description };
        OnButtonClick?.Invoke(parameters);
    }

    private void difficultyChanged(int value)
    {
        Difficulty difficulty;
        switch (value)
        {
            case 0:
                difficulty = Difficulty.Normal;
                break;
            case 1:
                difficulty = Difficulty.Ridiculous;
                break;
            default:
                difficulty = Difficulty.Normal;
                break;
        }

        OnDifficultyChanged?.Invoke(difficulty);
    }
}
