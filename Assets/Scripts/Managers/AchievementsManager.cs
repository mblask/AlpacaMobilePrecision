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
    Smash50,
    Smash100,
    SurviveAffSwitch,
    Survive10AffSwtiches,
    Have100Accuracy1Level,
    Have80PlusAccuracy8Levels,
    Have60PlusAccuracyAllGame,
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

    private AudioManager _audioManager;

    [SerializeField] private float _gameLevelStopwatch = 0.0f;
    private int _negativeCharactersKilled = 0;
    private List<AccuracyLevel> _accuracyPerAccuracyLevel = new List<AccuracyLevel>();
    private int _accuracyLevelsComplete = 0;
    private bool _affiliationSwitched = false;
    private int _numOfAffiliationSwitchesSurvived = 0;

    [Space]
    [SerializeField] private List<Achievement> _possibleAchievements;
    private List<AchievementType> _achievementsUnlocked = new List<AchievementType>();

    private bool _trackTime = false;
    private bool _trackCharacters = true;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _audioManager = AudioManager.Instance;

        LevelManager.Instance.OnGameReload += onGameReloadReset;
        GameManager.Instance.OnGameOver += onGameOverReset;
        GameManager.Instance.OnGameOverOnTime += onGameOverOnTimeReset;
        GameManager.Instance.OnQuitToMainMenu += stopTimeTracking;
        LevelManager.Instance.OnLoadLevel += trackAchievementsOnLoadLevel;
        Character.OnCharacterDestroyed += trackCharactersKilled;
        LevelManager.Instance.OnBeforeLoadLevel += trackAccuracyLevels;
    }

    private void Update()
    {
        if (_trackTime)
            _gameLevelStopwatch += Time.deltaTime;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnGameReload -= onGameReloadReset;
        GameManager.Instance.OnGameOver -= onGameOverReset;
        GameManager.Instance.OnGameOverOnTime -= onGameOverOnTimeReset;
        GameManager.Instance.OnQuitToMainMenu -= stopTimeTracking;
        LevelManager.Instance.OnLoadLevel -= trackAchievementsOnLoadLevel;
        Character.OnCharacterDestroyed -= trackCharactersKilled;
        LevelManager.Instance.OnBeforeLoadLevel -= trackAccuracyLevels;
    }

    public static void ResetAchievements()
    {
        _instance.resetAchievements();
        _instance.resetAllTrackers();
    }

    private void resetAchievements()
    {
        _achievementsUnlocked.Clear();
    }

    private void onGameReloadReset()
    {
        _affiliationSwitched = false;
    }

    private void onGameOverOnTimeReset()
    {
        _trackTime = false;
        _affiliationSwitched = false;
    }

    private void onGameOverReset()
    {
        _trackTime = false;
        _negativeCharactersKilled = 0;
        _affiliationSwitched = false;
    }

    private void resetAllTrackers()
    {
        _trackTime = false;
        _gameLevelStopwatch = 0.0f;
        _negativeCharactersKilled = 0;
        _accuracyLevelsComplete = 0;
        _accuracyPerAccuracyLevel.Clear();
        _affiliationSwitched = false;
        _numOfAffiliationSwitchesSurvived = 0;
    }

    public AchievementTrackers GetAchievementTrackers()
    {
        return new AchievementTrackers {
            GameLevelStopwatch = _gameLevelStopwatch,
            NegativeCharactersKilled = _negativeCharactersKilled,
            AccuracyLevelsComplete = _accuracyLevelsComplete,
            AccuracyPerAccuracyLevel = _accuracyPerAccuracyLevel,
            NumberOfAffiliationSwitches = _numOfAffiliationSwitchesSurvived
        };
    }

    private void trackAchievementsOnLoadLevel(int level)
    {
        if (!_trackTime)
            runTimeTracking();

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
            _audioManager?.PlaySFXClip(SFXClipType.AchievementScored);
        }
    }

    private string getAchievementName(AchievementType achievementType)
    {
        foreach (Achievement achievement in _possibleAchievements)
        {
            if (achievement.AchievementType.Equals(achievementType)) {
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

            switch (_numOfAffiliationSwitchesSurvived)
            {
                case 1:
                    //Survive an affiliation switch.
                    achievementUnlocked(AchievementType.SurviveAffSwitch);
                    break;

                case 10:
                    //Achievement scored! Survive ten affiliation switches.
                    achievementUnlocked(AchievementType.Survive10AffSwtiches);
                    break;

                default:
                    break;
            }

            _affiliationSwitched = false;
        }
    }

    private bool accuracyLevelComplete(AccuracyLevel accuracyLevel)
    {
        bool levelComplete = false;
        foreach (AccuracyLevel accLevel in _accuracyPerAccuracyLevel)
            if (accLevel.LevelNumber == accuracyLevel.LevelNumber)
            {
                levelComplete = true;
                break;
            }

        return levelComplete;
    }

    private void trackAccuracyLevels(AccuracyLevel accuracyLevel)
    {
        if (accuracyLevelComplete(accuracyLevel))
            return;

        _accuracyLevelsComplete++;
        _accuracyPerAccuracyLevel.Add(accuracyLevel);

        switch (_accuracyLevelsComplete)
        {
            case 1:
                if (_accuracyPerAccuracyLevel.TrueForAll(x => x.Accuracy == 1.0f))
                    //Have 1 accuracy level with a 100% accuracy
                    achievementUnlocked(AchievementType.Have100Accuracy1Level);
                break;

            case 8:
                if (_accuracyPerAccuracyLevel.TrueForAll(x => x.Accuracy >= 0.8f && x.Accuracy <= 1.0f))
                    //Have 5 accuracy levels with 80-100% accuracy, provided you already scored previous achievement
                    //if (_achievementsUnlocked.Contains(AchievementType.Have100Accuracy1Level))
                        achievementUnlocked(AchievementType.Have80PlusAccuracy8Levels);
                break;

            case 23:
                if (_accuracyPerAccuracyLevel.TrueForAll(x => x.Accuracy >= 0.6f && x.Accuracy <= 1.0f))
                    //Have 17 accuracy levels with 60-100% accuracy, provided you already scored previous achievement
                    //if (_achievementsUnlocked.Contains(AchievementType.Have80PlusAccuracy8Levels))
                        achievementUnlocked(AchievementType.Have60PlusAccuracyAllGame);
                break;

            default:
                break;
        }
    }

    private void runTimeTracking()
    {
        _trackTime = true;
    }

    private void stopTimeTracking()
    {
        _trackTime = false;
    }

    private void trackGameLevelTime(int level)
    {
        if (_gameLevelStopwatch == 0.0f)
            return;

        List<float> timeCheckList = new List<float> { 5.0f, 30.0f, 180.0f, 300.0f };

        switch (level)
        {
            case 4:
                if (_gameLevelStopwatch <= timeCheckList[0])
                    //Reach level 4 within 5 seconds
                    achievementUnlocked(AchievementType.Reach4);
                break;
            case 10:
                if (_gameLevelStopwatch <= timeCheckList[1])
                    //Reach level 10 within 30 seconds
                    achievementUnlocked(AchievementType.Reach10);
                break;
            case 20:
                if (_gameLevelStopwatch <= timeCheckList[2])
                    //Reach level 20 withing 3 minutes (180 seconds)
                    achievementUnlocked(AchievementType.Reach20);
                break;
            case 35:
                if (_gameLevelStopwatch <= timeCheckList[3])
                    //Reach final level within 5 minutes (300 seconds)
                    achievementUnlocked(AchievementType.Reach35);
                break;
            default:
                break;
        }
    }

    private void trackCharactersKilled(Character characterKilled)
    {
        if (!_trackCharacters)
            return;

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
                        //Kill 15 negative characters in a row.
                        achievementUnlocked(AchievementType.Smash15);
                        break;
                    case 50:
                        //Kill 50 negative characters in a row.
                        achievementUnlocked(AchievementType.Smash50);
                        break;
                    case 100:
                        //Achievement scored! Kill 100 negative characters in a row.
                        achievementUnlocked(AchievementType.Smash100);
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

    public void LoadAchievements(List<AchievementType> achievements)
    {
        if (achievements == null)
            return;

        if (_achievementsUnlocked == null)
            _achievementsUnlocked = new List<AchievementType>();

        foreach (AchievementType achievement in achievements)
            _achievementsUnlocked.Add(achievement);
    }

    public void LoadAchievementTrackers(AchievementTrackers achievementTrackers)
    {
        if (achievementTrackers == null)
            return;

        _gameLevelStopwatch = achievementTrackers.GameLevelStopwatch;
        _negativeCharactersKilled = achievementTrackers.NegativeCharactersKilled;
        _accuracyLevelsComplete = achievementTrackers.AccuracyLevelsComplete;

        if (_accuracyPerAccuracyLevel == null)
        {
            _accuracyPerAccuracyLevel = new List<AccuracyLevel>();
            _accuracyPerAccuracyLevel = achievementTrackers.AccuracyPerAccuracyLevel;
        }
        
        _numOfAffiliationSwitchesSurvived = achievementTrackers.NumberOfAffiliationSwitches;
    }

    public bool IsTrackingCharacters()
    {
        return _trackCharacters;
    }

    public void SetTrackingCharacters(bool value)
    {
        _trackCharacters = value;
    }
}
