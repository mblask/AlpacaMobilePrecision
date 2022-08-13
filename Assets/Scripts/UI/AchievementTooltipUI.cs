using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AchievementTooltipUI : MonoBehaviour
{
    private Transform _tooltipContainer;
    private RectTransform _rectTransform;
    private TextMeshProUGUI _achievementTitle;
    private TextMeshProUGUI _achivementDescription;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _tooltipContainer = transform.Find("Container");
        _achievementTitle = _tooltipContainer.Find("AchievementTitle").GetComponent<TextMeshProUGUI>();
        _achivementDescription = _tooltipContainer.Find("AchievementDescription").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        AchievementUI.OnButtonClick += setupTooltip;
    }

    private void OnDestroy()
    {
        AchievementUI.OnButtonClick -= setupTooltip;
    }

    public void OnTooltipButtonClick()
    {
        deactivateTooltip();
    }

    private void setupTooltip(AchievementUI achievement)
    {
        Vector2 newPosition = achievement.transform.position;
        _rectTransform.position = newPosition;

        _achievementTitle.SetText(achievement.GetAchievementName());
        _achivementDescription.SetText(achievement.GetAchievementDescription());

        activateTooltip();
    }

    private void activateTooltip()
    {
        _tooltipContainer.gameObject.SetActive(true);
    }

    private void deactivateTooltip()
    {
        _tooltipContainer.gameObject.SetActive(false);

        _achievementTitle.SetText("");
        _achivementDescription.SetText("");
    }
}
