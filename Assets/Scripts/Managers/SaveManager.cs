using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager
{
    public static void SaveProgress()
    {
        bool isFirstLaunch = GameManager.Instance.IsFirstLaunch();
        int level = LevelManager.Instance.GetLevel();
        List<WantedCharacter> wantedCharactersList = WantedListManager.Instance.GetUnlockedWantedCharactersList();
        
        List<AchievementType> achievementsList = AchievementsManager.Instance.GetAchievementsUnlocked();
        AchievementTrackers achievementTrackers = AchievementsManager.Instance.GetAchievementTrackers();

        Highscore highscore = GameManager.Instance.GetCurrentHighscore();

        GameProgress newGameProgress = new GameProgress { 
            FirstLaunch = isFirstLaunch,
            Level = level, 
            WantedCharactersList = wantedCharactersList, 
            AchievementsList = achievementsList, 
            AchievementTrackers = achievementTrackers,
            Highscore = highscore 
        };

        string gameProgressString = JsonUtility.ToJson(newGameProgress);

        string persistentDataPath = Application.persistentDataPath;
        string savePath = persistentDataPath + "/mobileprecgam.agsf";

        File.WriteAllText(savePath, gameProgressString);
    }

    public static void LoadProgress()
    {
        string persistentDataPath = Application.persistentDataPath;
        string savePath = persistentDataPath + "/mobileprecgam.agsf";

        if (File.Exists(savePath))
        {
            string loadString = File.ReadAllText(savePath);
            GameProgress gameProgress = JsonUtility.FromJson<GameProgress>(loadString);

            GameManager.Instance?.SetFirstLaunch(gameProgress.FirstLaunch);
            LevelManager.Instance?.SetLevel(gameProgress.Level);
            AchievementsManager.Instance?.LoadAchievements(gameProgress.AchievementsList);
            AchievementsManager.Instance?.LoadAchievementTrackers(gameProgress.AchievementTrackers);
            WantedListManager.Instance?.LoadWantedList(gameProgress.WantedCharactersList);
            GameManager.Instance?.LoadCurrentHighscore(gameProgress.Highscore);
        }
    }
}
