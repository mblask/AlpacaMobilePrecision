using System.Collections.Generic;

[System.Serializable]
public class AchievementTrackers
{
    public float GameLevelStopwatch;
    public List<int> LevelsReached;
    public int NegativeCharactersKilled;
    public int AccuracyLevelsComplete;
    public List<AccuracyLevel> AccuracyPerAccuracyLevel;
    public int NumberOfAffiliationSwitches;
}
