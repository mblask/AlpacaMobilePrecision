using System.Collections.Generic;

[System.Serializable]
public class GameProgress
{
    public bool FirstLaunch;
    public Difficulty Difficulty;
    public int Level;
    public List<Checkpoint> CheckpointsList;
    public List<WantedCharacter> WantedCharactersList;
    public List<AchievementType> AchievementsList;
    public AchievementTrackers AchievementTrackers;
    public Highscore Highscore;
}
