using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AccuracyNeededUI : MonoBehaviour
{
    private TextMeshProUGUI _accuracyNeededNumber;
    private TextMeshProUGUI _accuracyNeededText;

    private void Awake()
    {
        _accuracyNeededNumber = transform.Find("AccuracyNeededNumber").GetComponent<TextMeshProUGUI>();
        _accuracyNeededText = transform.Find("AccuracyNeededText").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        showAccuracyNeeded(false);
    }

    private void updateAccuracy(float value)
    {
        int intAccuracy = (int)value;
        _accuracyNeededNumber.SetText(intAccuracy.ToString());
    }

    private void showAccuracyNeeded(bool value)
    {
        _accuracyNeededNumber.gameObject.SetActive(value);
        _accuracyNeededText.gameObject.SetActive(value);
    }
}
