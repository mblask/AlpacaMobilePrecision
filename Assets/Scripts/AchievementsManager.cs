using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    private int _negativeCharactersKilled = 0;
    private float _gameLevelStopwatch = 0.0f;
    private List<float> _accuracyPerAccuracyLevel = new List<float>();
    private int _accuracyLevelsComplete = 0;
    private bool _affiliationSwitched = false;
    private int _numOfAffiliationSwitchesSurvived = 0;

    private void Start()
    {
        LevelManager.Instance.OnLoadLevel += achievementsOnLoadLevel;
        Character.OnCharacterDestroyed += trackCharactersKilled;
        LevelManager.Instance.OnBeforeLoadLevel += trackAccuracyLevels;
        LevelManager.Instance.OnGameReload += resetTrackers;
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

    private void trackAffiliationSwitches()
    {
        if (_affiliationSwitched)
        {
            _numOfAffiliationSwitchesSurvived++;
            if (_numOfAffiliationSwitchesSurvived == 1)
                Debug.Log("Survived: " + _numOfAffiliationSwitchesSurvived + " affiliation switch");
            else
                Debug.Log("Survived: " + _numOfAffiliationSwitchesSurvived + " affiliation switches");

            switch (_numOfAffiliationSwitchesSurvived)
            {
                case 1:
                    Debug.Log("Achievement scored! Survive an affiliation switch.");
                    break;
                case 3:
                    Debug.Log("Achievement scored! Survive three affiliation switches.");
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
        Debug.Log("Accuracy: " + accuracy + ", levels complete: " + _accuracyLevelsComplete);

        switch (_accuracyLevelsComplete)
        {
            case 3:
                if (_accuracyPerAccuracyLevel.TrueForAll(x => x >= 0.8f && x < 1.0f))
                    Debug.Log("Achievement scored! Have 2 levels in a row accuracy higher than 80%.");

                if (_accuracyPerAccuracyLevel.TrueForAll(x => x == 1.0f))
                    Debug.Log("Achievement scored! Have 2 levels in a row accuracy of 100%.");
                break;
            case 15:
                if (_accuracyPerAccuracyLevel.TrueForAll(x => x == 1.0f))
                    Debug.Log("Achievement scored! In all levels have 100% accuracy.");
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
                    Debug.Log("Achievement scored! Reach level 4 within 5 seconds.");
                break;
            case 10:
                if (_gameLevelStopwatch <= 30.0f)
                    Debug.Log("Achievement scored! Reach level 10 within 30 seconds.");
                break;
            case 20:
                if (_gameLevelStopwatch <= 60.0f)
                    Debug.Log("Achievement scored! Reach level 20 within 1 minute.");
                break;
            case 35:
                if (_gameLevelStopwatch <= 180.0f)
                    Debug.Log("Achievement scored! Reach last level within 3 minutes.");
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
                        Debug.Log("Achievement scored! Kill 15 negative characters in a row.");
                        break;
                    case 30:
                        Debug.Log("Achievement scored! Kill 30 negative characters in a row.");
                        break;
                    case 50:
                        Debug.Log("Achievement scored! Kill 50 negative characters in a row.");
                        break;
                    default:
                        break;
                }
                break;
            case CharacterType.Neutral:
                _affiliationSwitched = true;
                break;
            default:
                break;
        }
    }
}
