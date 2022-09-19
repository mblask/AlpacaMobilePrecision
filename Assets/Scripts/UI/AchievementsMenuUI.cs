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
    private Transform _gameVictoriesContainer;
    private TextMeshProUGUI _numberOfGameVictoriesText;

    private GameManager _gameManager;

    private void Awake()
    {
        _achievementsUIContainer = transform.Find("Container").Find("Achievements").Find("AchievementsContainer");
        _highscoreNumberText = transform.Find("Container").Find("Highscore").Find("HighscoreNumber").GetComponent<TextMeshProUGUI>();
        _highscoreDateText = transform.Find("Container").Find("Highscore").Find("HighscoreDate").GetComponent<TextMeshProUGUI>();
        _gameVictoriesContainer = transform.Find("Container").Find("GamePassed");
        _numberOfGameVictoriesText = _gameVictoriesContainer.Find("NumberText").GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        _gameManager = GameManager.Instance;
        updateAchievements();
    }

    private void updateAchievements()
    {
        updateHighscoreNumber(_gameManager.GetCurrentHighscore());
        updateGamePassedNumberTimes(_gameManager.GetNumberOfVictories());

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

    private void updateHighscoreNumber(Highscore highscore)
    {
        if (highscore == null)
            return;

        _highscoreNumberText.SetText(highscore.Score.ToString("F0"));
        _highscoreDateText.SetText(highscore.Date);
    }

    private void updateGamePassedNumberTimes(int numberOfPasses)
    {
        if (numberOfPasses > 0)
        {
            _gameVictoriesContainer.gameObject.SetActive(true);

            string timeString = (numberOfPasses > 1) ? " times" : " time";
            _numberOfGameVictoriesText.SetText(numberOfPasses.ToString() + timeString);
        }
    }
}
