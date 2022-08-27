using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AchievementType
{
    Reach4,
    Reach10,
    Reach20,
    Reach35,
    Smash15,
    Smash30,
    Smash50,
    SurviveAffSwitch,
    Survive3AffSwtiches,
    Have80Accuracy3Levels,
    Have100Accuracy3Levels,
    Have100AccuracyAllGame,
}

[Serializable]
public class Achievement
{
    public string AchievementName;
    [TextArea] public string AchievementDescription;
    public Sprite AchievementSprite;
    public AchievementType AchievementType;
}

public class AchievementsManager : MonoBehaviour
{
    private static AchievementsManager _instance;

    public static AchievementsManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public Action<string> OnAchievementScored;

    private int _negativeCharactersKilled = 0;
    private float _gameLevelStopwatch = 0.0f;
    private List<float> _accuracyPerAccuracyLevel = new List<float>();
    private int _accuracyLevelsComplete = 0;
    private bool _affiliationSwitched = false;
    private int _numOfAffiliationSwitchesSurvived = 0;
    
    [SerializeField] private List<Achievement> _possibleAchievements;
    private List<AchievementType> _achievementsUnlocked;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        LevelManager.Instance.OnLoadLevel += achievementsOnLoadLevel;
        Character.OnCharacterDestroyed += trackCharactersKilled;
        LevelManager.Instance.OnBeforeLoadLevel += trackAccuracyLevels;
        LevelManager.Instance.OnGameReload += resetTrackers;
        GameManager.Instance.OnGameOver += resetTrackers;
    }

    private void Update()
    {
        _gameLevelStopwatch += Time.deltaTime;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnLoadLevel -= achievementsOnLoadLevel;
        Character.OnCharacterDestroyed -= trackCharactersKilled;
        LevelManager.Instance.OnBeforeLoadLevel -= trackAccuracyLevels;
        LevelManager.Instance.OnGameReload -= resetTrackers;
        GameManager.Instance.OnGameOver -= resetTrackers;
    }

    private void resetTrackers()
    {
        _negativeCharactersKilled = 0;
        _gameLevelStopwatch = 0.0f;
        _accuracyLevelsComplete = 0;
        _accuracyPerAccuracyLevel.Clear();
        _affiliationSwitched = false;
    }

    private void achievementsOnLoadLevel(int level)
    {
        trackGameLevelTime(level);
        trackAffiliationSwitches();
    }

    public List<AchievementType> GetAchievementsUnlocked()
    {
        return _achievementsUnlocked;
    }

    public List<Achievement> GetAchievementsList()
    {
        return _possibleAchievements;
    }

    private void achievementUnlocked(AchievementType achievementType)
    {
        if (_achievementsUnlocked == null)
            _achievementsUnlocked = new List<AchievementType>();

        if (!_achievementsUnlocked.Contains(achievementType))
        {
            _achievementsUnlocked.Add(achievementType);
            OnAchievementScored?.Invoke(getAchievementName(achievementType));
        }
    }

    private string getAchievementName(AchievementType achievementType)
    {
        foreach (Achievement achievement in _possibleAchievements)
        {
            if (achievement.AchievementType.Equals(achievementType)){
                return achievement.AchievementName;
            }
        }

        return "";
    }

    private void trackAffiliationSwitches()
    {
        if (_affiliationSwitched)
        {
            _numOfAffiliationSwitchesSurvived++;
            //if (_numOfAffiliationSwitchesSurvived == 1)
            //    Debug.Log("Survived: " + _numOfAffiliationSwitchesSurvived + " affiliation switch");
            //else
            //    Debug.Log("Survived: " + _numOfAffiliationSwitchesSurvived + " affiliation switches");

            switch (_numOfAffiliationSwitchesSurvived)
            {
                case 1:
                    //Debug.Log("Achievement scored! Survive an affiliation switch.");
                    achievementUnlocked(AchievementType.SurviveAffSwitch);
                    break;
                case 3:
                    //Debug.Log("Achievement scored! Survive three affiliation switches.");
                    achievementUnlocked(AchievementType.Survive3AffSwtiches);
                    break;
                default:
                    break;
            }
        }
    }

    private void trackAccuracyLevels(float accuracy)
    {
        _accuracyLevelsComplete++;
        _accuracyPerAccuracyLevel.Add(accuracy);
        //Debug.Log("Accuracy: " + accuracy + ", levels complete: " + _accuracyLevelsComplete);

        switch (_accuracyLevelsComplete)
        {
            case 3:
                if (_accuracyPerAccuracyLevel.TrueForAll(x => x >= 0.8f && x < 1.0f))
                {
                    //Debug.Log("Achievement scored! Have 3 levels in a row accuracy higher than 80%.");
                    achievementUnlocked(AchievementType.Have80Accuracy3Levels);
                }

                if (_accuracyPerAccuracyLevel.TrueForAll(x => x == 1.0f))
                {
                    //Debug.Log("Achievement scored! Have 3 levels in a row accuracy of 100%.");
                    achievementUnlocked(AchievementType.Have100Accuracy3Levels);
                }
                break;
            case 15:
                if (_accuracyPerAccuracyLevel.TrueForAll(x => x == 1.0f))
                {
                    //Debug.Log("Achievement scored! In all levels have 100% accuracy.");
                    achievementUnlocked(AchievementType.Have100AccuracyAllGame);
                }
                break;
            default:
                break;
        }
    }

    private void trackGameLevelTime(int level)
    {
        switch (level)
        {
            case 4:
                if (_gameLevelStopwatch <= 5.0f)
                {
                    //Debug.Log("Achievement scored! Reach level 4 within 5 seconds.");
                    achievementUnlocked(AchievementType.Reach4);
                }
                break;
            case 10:
                if (_gameLevelStopwatch <= 60.0f)
                {
                    //Debug.Log("Achievement scored! Reach level 10 within 60 seconds.");
                    achievementUnlocked(AchievementType.Reach10);
                }
                break;
            case 20:
                if (_gameLevelStopwatch <= 180.0f)
                {
                    //Debug.Log("Achievement scored! Reach level 20 within 3 minutes.");
                    achievementUnlocked(AchievementType.Reach20);
                }
                break;
            case 35:
                if (_gameLevelStopwatch <= 300.0f)
                {
                    //Debug.Log("Achievement scored! Reach last level within 5 minutes.");
                    achievementUnlocked(AchievementType.Reach35);
                }
                break;
            default:
                break;
        }
    }

    private void trackCharactersKilled(Character characterKilled)
    {
        switch (characterKilled.GetCharacterType())
        {
            case CharacterType.Positive:
                _negativeCharactersKilled = 0;
                break;
            case CharacterType.Negative:
                _negativeCharactersKilled++;
                switch (_negativeCharactersKilled)
                {
                    case 15:
                        //Debug.Log("Achievement scored! Kill 15 negative characters in a row.");
                        achievementUnlocked(AchievementType.Smash15);
                        break;
                    case 30:
                        //Debug.Log("Achievement scored! Kill 30 negative characters in a row.");
                        achievementUnlocked(AchievementType.Smash30);
                        break;
                    case 50:
                        //Debug.Log("Achievement scored! Kill 50 negative characters in a row.");
                        achievementUnlocked(AchievementType.Smash50);
                        break;
                    default:
                        break;
                }
                break;
            case CharacterType.AffiliationTrigger:
                _affiliationSwitched = true;
                break;
            default:
                break;
        }
    }
}
