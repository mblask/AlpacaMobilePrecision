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
    Smash40,
    Smash70,
    SurviveAffSwitch,
    Survive3AffSwtiches,
    Have100Accuracy1Level,
    Have80PlusAccuracy5Levels,
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

    private int _negativeCharactersKilled = 0;
    [SerializeField] private float _gameLevelStopwatch = 0.0f;
    private List<float> _accuracyPerAccuracyLevel = new List<float>();
    private int _accuracyLevelsComplete = 0;
    private bool _affiliationSwitched = false;
    private int _numOfAffiliationSwitchesSurvived = 0;

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

        LevelManager.Instance.OnLoadLevel += achievementsOnLoadLevel;
        Character.OnCharacterDestroyed += trackCharactersKilled;
        LevelManager.Instance.OnBeforeLoadLevel += trackAccuracyLevels;
        LevelManager.Instance.OnGameReload += resetTrackers;
        GameManager.Instance.OnGameOver += resetTrackers;
        GameManager.Instance.OnQuitToMainMenu += stopTimeTracking;
    }

    private void Update()
    {
        if (_trackTime)
            _gameLevelStopwatch += Time.deltaTime;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnLoadLevel -= achievementsOnLoadLevel;
        Character.OnCharacterDestroyed -= trackCharactersKilled;
        LevelManager.Instance.OnBeforeLoadLevel -= trackAccuracyLevels;
        LevelManager.Instance.OnGameReload -= resetTrackers;
        GameManager.Instance.OnGameOver -= resetTrackers;
        GameManager.Instance.OnQuitToMainMenu -= stopTimeTracking;
    }

    private void resetTrackers()
    {
        _trackTime = false;
        _gameLevelStopwatch = 0.0f;
        _negativeCharactersKilled = 0;
        _accuracyLevelsComplete = 0;
        _accuracyPerAccuracyLevel.Clear();
        _affiliationSwitched = false;
    }

    public AchievementTrackers GetAchievementTrackers()
    {
        return new AchievementTrackers {
            GameLevelStopwatch = _gameLevelStopwatch,
            NegativeCharactersKilled = _negativeCharactersKilled,
            AccuracyLevelsComplete = _accuracyLevelsComplete,
            AccuracyPerAccuracyLevel = _accuracyPerAccuracyLevel,
            AffiliationSwitched = _affiliationSwitched
        };
    }

    private void achievementsOnLoadLevel(int level)
    {
        if (level == 1)
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

                case 3:
                    //Achievement scored! Survive three affiliation switches.
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

        /*
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
            case 17:
                if (_accuracyPerAccuracyLevel.TrueForAll(x => x == 1.0f))
                {
                    //Debug.Log("Achievement scored! In all levels have 100% accuracy.");
                    achievementUnlocked(AchievementType.Have100AccuracyAllGame);
                }
                break;
            default:
                break;
        }
        */

        //***IF IT WORKS RENAME THE ENUMS FOR ACCURACY!!!!!***
        switch (_accuracyLevelsComplete)
        {
            case 1:
                if (_accuracyPerAccuracyLevel.TrueForAll(x => x == 1.0f))
                    //Have 1 accuracy level with a 100% accuracy
                    achievementUnlocked(AchievementType.Have100Accuracy1Level);
                break;

            case 5:
                if (_accuracyPerAccuracyLevel.TrueForAll(x => x >= 0.8f && x <= 1.0f))
                    //Have 5 accuracy levels with 80-100% accuracy, provided you already scored previous achievement
                    if (_achievementsUnlocked.Contains(AchievementType.Have100Accuracy1Level))
                        achievementUnlocked(AchievementType.Have80PlusAccuracy5Levels);
                break;

            case 17:
                if (_accuracyPerAccuracyLevel.TrueForAll(x => x >= 0.6f && x <= 1.0f))
                    //Have 17 accuracy levels with 60-100% accuracy, provided you already scored previous achievement
                    if (_achievementsUnlocked.Contains(AchievementType.Have80PlusAccuracy5Levels))
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
        List<float> timeCheckList = new List<float> { 5.0f, 40.0f, 180.0f, 300.0f };

        switch (level)
        {
            case 4:
                if (_gameLevelStopwatch <= timeCheckList[0])
                    //Reach level 4 within 5 seconds
                    achievementUnlocked(AchievementType.Reach4);
                break;
            case 10:
                if (_gameLevelStopwatch <= timeCheckList[1])
                    //Reach level 10 within 40 seconds
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
                    case 40:
                        //Kill 30 negative characters in a row.
                        achievementUnlocked(AchievementType.Smash40);
                        break;
                    case 70:
                        //Achievement scored! Kill 50 negative characters in a row.
                        achievementUnlocked(AchievementType.Smash70);
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
        if (_achievementsUnlocked == null)
            _achievementsUnlocked = new List<AchievementType>();

        foreach (AchievementType achievement in achievements)
            _achievementsUnlocked.Add(achievement);
    }

    public void LoadAchievementTrackers(AchievementTrackers achievementTrackers)
    {
        _gameLevelStopwatch = achievementTrackers.GameLevelStopwatch;
        _negativeCharactersKilled = achievementTrackers.NegativeCharactersKilled;
        _accuracyLevelsComplete = achievementTrackers.AccuracyLevelsComplete;

        if (_accuracyPerAccuracyLevel == null)
        {
            _accuracyPerAccuracyLevel = new List<float>();
            _accuracyPerAccuracyLevel = achievementTrackers.AccuracyPerAccuracyLevel;
        }
        
        _affiliationSwitched = achievementTrackers.AffiliationSwitched;
    }

    public bool GetTrackCharacters()
    {
        return _trackCharacters;
    }

    public void SetTrackCharacters(bool value)
    {
        _trackCharacters = value;
    }
}
