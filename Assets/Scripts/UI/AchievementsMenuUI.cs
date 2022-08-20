using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private void Start()
    {
        updateHighscoreNumber(0.0f);
    }

    private void updateAchievements()
    {
        List<AchievementType> achievementsUnlocked = AchievementsManager.Instance?.GetAchievementsUnlocked();

        if (achievementsUnlocked == null || achievementsUnlocked.Count == 0)
            return;

        AchievementUI[] achievementsArray = _achievementsUIContainer.GetComponentsInChildren<AchievementUI>();

        if (achievementsArray == null)
            return;

        foreach (AchievementUI achievement in achievementsArray)
        {
            foreach (AchievementType type in achievementsUnlocked)
            {
                if (achievement.GetAchievementType().Equals(type))
                    if (achievement.IsLocked())
                        achievement.SetLocked(false);
            }
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
            dateString = "On " + dateTime.Date + " " + dateTime.Month + " " + dateTime.Year;
        }

        _highscoreDateText.SetText(dateString);
    }
}
