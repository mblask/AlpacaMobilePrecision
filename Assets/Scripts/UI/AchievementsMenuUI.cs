using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;
using TMPro;

public class AchievementsMenuUI : MonoBehaviour
{
    private Transform _achievementsUIContainer;

    private TextMeshProUGUI _highscoreNumberText;
    private TextMeshProUGUI _highscoreDateText;

    private void Awake()
    {
        _achievementsUIContainer = transform.Find("Container").Find("Achievements").Find("AchievementsContainer");
        _highscoreNumberText = transform.Find("Container").Find("Highscore").Find("HighscoreNumber").GetComponent<TextMeshProUGUI>();
        _highscoreDateText = transform.Find("Container").Find("Highscore").Find("HighscoreDate").GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        updateAchievements();
    }

    private void updateAchievements()
    {
        updateHighscoreNumber(GameManager.Instance.GetCurrentHighscore());

        if (_achievementsUIContainer.childCount != 0)
        {
            for (int i = 0; i < _achievementsUIContainer.childCount; i++)
                Destroy(_achievementsUIContainer.GetChild(i).gameObject);
        }

        List<Achievement> achievementsList = AchievementsManager.Instance?.GetAchievementsList();
        List<AchievementType> unlockedAchievements = AchievementsManager.Instance?.GetAchievementsUnlocked();

        if (achievementsList == null || achievementsList.Count == 0)
            return;

        for (int i = 0; i < achievementsList.Count; i++)
        {
            AchievementUI achievementUI = Instantiate(GameAssets.Instance.AchievementUI, _achievementsUIContainer).GetComponent<AchievementUI>();
            achievementUI.SetupUI(achievementsList[i]);

            if (unlockedAchievements == null || unlockedAchievements.Count == 0)
                continue;

            if (achievementUI.IsLocked())
                achievementUI.SetLocked(!unlockedAchievements.Contains(achievementUI.GetAchievementType()));
        }
    }

    private void updateHighscoreNumber(float highscore)
    {
        _highscoreNumberText.SetText(highscore.ToString("F0"));
        updateHighscoreDate(highscore);
    }

    private void updateHighscoreDate(float highscore = 0.0f)
    {
        string dateString;
        if (highscore == 0.0f)
            dateString = "On 1 January 2000";
        else
        {
            System.DateTime dateTime = System.DateTime.Now;
            dateString = "On " + dateTime.Day + " " + Utilities.GetMonthName(dateTime.Month) + " " + dateTime.Year;
        }

        _highscoreDateText.SetText(dateString);
    }
}
