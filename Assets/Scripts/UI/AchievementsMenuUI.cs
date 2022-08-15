using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsMenuUI : MonoBehaviour
{
    private Transform _achievementsUIContainer;

    private void Awake()
    {
        _achievementsUIContainer = transform.Find("AchievementsContainer");
    }

    void OnEnable()
    {
        updateAchievements();
    }

    private void updateAchievements()
    {
        List<AchievementType> achievementsUnlocked = AchievementsManager.Instance.GetAchievementsUnlocked();

        if (achievementsUnlocked.Count == 0)
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
}
