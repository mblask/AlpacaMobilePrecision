using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RemainingNegatives : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = transform.Find("RemainingNegativesText").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        LevelManager.Instance.OnSendNegativeCharactersNumber += updateText;
    }

    private void OnDisable()
    {
        LevelManager.Instance.OnSendNegativeCharactersNumber -= updateText;
    }

    private void updateText(int numberOfNegatives)
    {
        string negativesLeftText = "Negatives left: ";
        _text.SetText(negativesLeftText + numberOfNegatives.ToString());
    }
}
