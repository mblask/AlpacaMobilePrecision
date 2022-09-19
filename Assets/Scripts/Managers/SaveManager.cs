using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager
{
    public static void SaveProgress()
    {
        //Debug.Log("Save progress");
        int level = LevelManager.Instance.GetLevel();
        List<WantedCharacter> wantedCharactersList = WantedListManager.Instance.GetUnlockedWantedCharactersList();
        List<AchievementType> achievementsList = AchievementsManager.Instance.GetAchievementsUnlocked();
        Highscore highscore = GameManager.Instance.GetCurrentHighscore();

        GameProgress newGameProgress = new GameProgress { Level = level, WantedCharactersList = wantedCharactersList, AchievementsList = achievementsList, Highscore = highscore };

        string gameProgressString = JsonUtility.ToJson(newGameProgress);
        //Debug.Log(gameProgressString);

        //string persistentDataPath = Application.dataPath;
        string persistentDataPath = Application.persistentDataPath;
        string savePath = persistentDataPath + "/mobileprecgam.agsf";

        File.WriteAllText(savePath, gameProgressString);
    }

    public static void LoadProgress()
    {
        //Debug.Log("Load progress");

        //string persistentDataPath = Application.dataPath;
        string persistentDataPath = Application.persistentDataPath;
        string savePath = persistentDataPath + "/mobileprecgam.agsf";

        if (File.Exists(savePath))
        {
            string loadString = File.ReadAllText(savePath);
            GameProgress gameProgress = JsonUtility.FromJson<GameProgress>(loadString);

            LevelManager.Instance?.SetLevel(gameProgress.Level);
            AchievementsManager.Instance?.LoadAchievements(gameProgress.AchievementsList);
            WantedListManager.Instance?.LoadWantedList(gameProgress.WantedCharactersList);
            GameManager.Instance?.LoadCurrentHighscore(gameProgress.Highscore);
        }
    }
}
