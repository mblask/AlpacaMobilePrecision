using UnityEngine;

[System.Serializable]
public class Achievement
{
    public string AchievementName;
    [TextArea] public string AchievementDescription;
    public Sprite AchievementSprite;
    public AchievementType AchievementType;
}
