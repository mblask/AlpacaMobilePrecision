using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class DifficultyUI : MonoBehaviour
{
    private TMP_Dropdown _dropdown;
    public static Action<Difficulty> OnDifficultyChanged;

    private void Awake()
    {
        _dropdown = transform.Find("DifficultyDropdown").GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        _dropdown.onValueChanged.AddListener(difficultyChanged);
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
