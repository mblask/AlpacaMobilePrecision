using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
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
        WantedCharacterUI.OnButtonClick += setupTooltip;
        DifficultyUI.OnButtonClick += setupTooltip;
    }

    private void OnDestroy()
    {
        AchievementUI.OnButtonClick -= setupTooltip;
        WantedCharacterUI.OnButtonClick -= setupTooltip;
        DifficultyUI.OnButtonClick -= setupTooltip;
    }

    public void OnTooltipButtonClick()
    {
        deactivateTooltip();
    }

    private void setupTooltip(TooltipParameters parameters)
    {
        Vector2 newPosition = parameters.Position;
        _rectTransform.position = newPosition;

        _achievementTitle.SetText(parameters.Title);
        _achivementDescription.SetText(parameters.Description);

        activateTooltip();
    }

    public void TooltipSetActive(bool value)
    {
        if (value)
            activateTooltip();
        else
            deactivateTooltip();
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
