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
        List<Checkpoint> checkpointsList = LevelManager.Instance.GetCheckpoints();

        List<WantedCharacter> wantedCharactersList = WantedListManager.Instance.GetUnlockedWantedCharactersList();
        
        List<AchievementType> achievementsList = AchievementsManager.Instance.GetAchievementsUnlocked();
        AchievementTrackers achievementTrackers = AchievementsManager.Instance.GetAchievementTrackers();

        Difficulty difficulty = GameManager.Instance.GetDifficulty();
        Highscore highscore = GameManager.Instance.GetCurrentHighscore();

        GameProgress newGameProgress = new GameProgress {
            FirstLaunch = isFirstLaunch,
            Difficulty = difficulty,
            Level = level,
            CheckpointsList = checkpointsList,
            WantedCharactersList = wantedCharactersList, 
            AchievementsList = achievementsList, 
            AchievementTrackers = achievementTrackers,
            Highscore = highscore 
        };

        string gameProgressString = JsonUtility.ToJson(newGameProgress);
        /*
        string persistentDataPath = Application.persistentDataPath;
        string savePath = persistentDataPath + "/mobileprecgam.agsf";
        */

        string dataPath;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.WindowsEditor:
                dataPath = Application.persistentDataPath;
                break;
            case RuntimePlatform.WindowsPlayer:
                dataPath = Application.dataPath;
                break;
            default:
                dataPath = "";
                break;
        }

        string savePath = dataPath + "/mobileprecgam.agsf";

        File.WriteAllText(savePath, gameProgressString);
    }

    public static void LoadProgress()
    {
        string dataPath;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.WindowsEditor:
                dataPath = Application.persistentDataPath;
                break;
            case RuntimePlatform.WindowsPlayer:
                dataPath = Application.dataPath;
                break;
            default:
                dataPath = "";
                break;
        }

        string savePath = dataPath + "/mobileprecgam.agsf";
        
        //string persistentDataPath = Application.persistentDataPath;
        //string savePath = persistentDataPath + "/mobileprecgam.agsf";

        if (File.Exists(savePath))
        {
            string loadString = File.ReadAllText(savePath);
            GameProgress gameProgress = JsonUtility.FromJson<GameProgress>(loadString);

            GameManager.Instance?.SetFirstLaunch(gameProgress.FirstLaunch);
            GameManager.Instance.SetDifficulty(gameProgress.Difficulty);
            LevelManager.Instance?.SetLevel(gameProgress.Level);
            LevelManager.Instance?.SetCheckpoints(gameProgress.CheckpointsList);
            AchievementsManager.Instance?.LoadAchievements(gameProgress.AchievementsList);
            AchievementsManager.Instance?.LoadAchievementTrackers(gameProgress.AchievementTrackers);
            WantedListManager.Instance?.LoadWantedList(gameProgress.WantedCharactersList);
            GameManager.Instance?.LoadCurrentHighscore(gameProgress.Highscore);
        }
    }
}
