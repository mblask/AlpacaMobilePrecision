using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AlpacaMyGames;

public enum GameOverType
{
    Victory,
    Failure,
}

public enum Difficulty
{
    Normal,
    Ridiculous,
}

public class GameManager : MonoBehaviour
{
    public event Action OnFirstLaunch;
    public event Action<int> OnScoreUpdate;
    public event Action OnGameOver;
    public event Action<Highscore> OnGameOverSendFinalScore;
    public event Action OnGameOverOnTime;
    public event Action OnGameOverOnScore;
    public event Action OnQuitToMainMenu;
    public event Action OnWorldDestruction;
    public event Action<float> OnSetLightingIntensity;
    public event Action<Difficulty> OnSetDifficulty;
    public event Action<bool> OnGamePaused;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private AudioManager _audioManager;

    [Header("Score Points")]
    [SerializeField] private int _score = 0;
    private Highscore _currentHighscore = new Highscore();

    private int _posCharHit = -3;
    private int _negCharHit = 4;
    private int _fragileObstHit = -1;

    private bool _affiliationChangedThisLevel = false;

    [Header("Read-only")]
    [SerializeField] private bool _updateScore = true;

    private int _numberOfGameVictories = 0;
    private bool _gamePaused = false;
    private Difficulty _difficulty;

    private bool _firstLaunch = true;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        Character.OnCharacterDestroyed += updateScoreByCharacterType;
        Obstacle.OnObstacleDestroy += updateScoreByObstacle;
        AffiliationTrigger.OnAffiliationTriggerHit += affiliationChangedThisLevel;
        LevelManager.Instance.OnLoadLevel += gameManager_onLoadLevel;
        LevelManager.Instance.OnGameReload += resetScore;
        LevelManager.Instance.OnResetUI += resetScore;
        LevelManager.Instance.OnGamePassed += gamePassed;
        TimeManager.Instance.OnTimeIsOut += gameOverOnTime;
        DifficultyUI.OnDifficultyChanged += SetDifficulty;

        _audioManager = AudioManager.Instance;

        resetScore();

        SaveManager.LoadProgress();

        onFirstLaunch();
    }

    private void OnDestroy()
    {
        Character.OnCharacterDestroyed -= updateScoreByCharacterType;
        Obstacle.OnObstacleDestroy -= updateScoreByObstacle;
        AffiliationTrigger.OnAffiliationTriggerHit -= affiliationChangedThisLevel;
        LevelManager.Instance.OnLoadLevel -= gameManager_onLoadLevel;
        LevelManager.Instance.OnGameReload -= resetScore;
        LevelManager.Instance.OnResetUI -= resetScore;
        LevelManager.Instance.OnGamePassed -= gamePassed;
        TimeManager.Instance.OnTimeIsOut -= gameOverOnTime;
        DifficultyUI.OnDifficultyChanged -= SetDifficulty;
    }

    public Difficulty GetDifficulty()
    {
        return _difficulty;
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        if (_difficulty.Equals(difficulty))
            return;

        _difficulty = difficulty;
        OnSetDifficulty?.Invoke(difficulty);

        setLightingIntensity(difficulty);
    }

    private void setLightingIntensity(Difficulty difficulty)
    {
        float normalIntensity = 1.0f;
        float ridiculousIntensity = 0.25f;

        switch (difficulty)
        {
            case Difficulty.Normal:
                OnSetLightingIntensity?.Invoke(normalIntensity);
                break;
            case Difficulty.Ridiculous:
                OnSetLightingIntensity?.Invoke(ridiculousIntensity);
                break;
            default:
                OnSetLightingIntensity?.Invoke(normalIntensity);
                break;
        }
    }

    private void onFirstLaunch()
    {
        if (_firstLaunch)
        {
            OnFirstLaunch?.Invoke();
            _firstLaunch = false;
        }
    }

    public bool IsFirstLaunch()
    {
        return _firstLaunch;
    }

    public void SetFirstLaunch(bool value)
    {
        _firstLaunch = value;
    }

    public void ResetGameProgress()
    {
        //reset difficulty
        SetDifficulty(Difficulty.Normal);

        //reset level manager
        LevelManager.ResetGameSettings();

        //reset achievements
        AchievementsManager.ResetAchievements();

        //reset wanted kills
        WantedListManager.ResetWantedList();
        
        //reset current highscore
        resetCurrentHighscore();

        //save reseted state in save manager
        SaveManager.SaveProgress();
    }

    private void updateScoreByObstacle(Obstacle obstacle)
    {
        updateScore(_fragileObstHit);
    }

    private void updateScoreByCharacterType(Character character)
    {
        if (!_updateScore)
            return;

        switch (character.GetCharacterType())
        {
            case CharacterType.Positive:
                if (!_affiliationChangedThisLevel)
                    updateScore(_posCharHit);
                else
                {
                    gameOver(GameOverType.Failure);
                    _score = 0;
                    destroyWorld();
                }
                break;
            case CharacterType.Negative:
                updateScore(_negCharHit);
                break;
            default:
                updateScore(0);
                break;
        }
    }
    
    private void updateScore(int scoreIncrement)
    {
        if (!_updateScore)
            return;

        _score += scoreIncrement;

        if (_score < 0)
            gameOverOnScore();

        OnScoreUpdate?.Invoke(_score);
    }
    
    private void resetScore()
    {
        _score = 0;
        OnScoreUpdate?.Invoke(_score);
    }
    
    public void SetUpdateScore(bool value)
    {
        _updateScore = value;
    }

    private void gameManager_onLoadLevel(int level)
    {
        resetAffiliationChanged();
    }
    
    private void gameOver(GameOverType gameOverType, bool onTime = false)
    {
        switch (gameOverType)
        {
            case GameOverType.Victory:
                _audioManager?.PlaySFXClip(SFXClipType.Victory);
                _numberOfGameVictories++;
                Debug.LogError("Victory!!");
                break;
            case GameOverType.Failure:
                _audioManager?.PlaySFXClip(SFXClipType.Failure);
                Debug.LogError("Game Over");
                break;
            default:
                break;
        }

        float finalScore = calculateFinalScore();

        if (!onTime)
            OnGameOver?.Invoke();

        EvaluateNewHighscore(finalScore);
    }

    public int GetNumberOfVictories()
    {
        return _numberOfGameVictories;
    }

    private void gameOverOnTime()
    {
        gameOver(GameOverType.Failure);
        OnGameOverOnTime?.Invoke();
    }

    private void gameOverOnScore()
    {
        gameOver(GameOverType.Failure);
        OnGameOverOnScore?.Invoke();
        OnGameOverSendFinalScore?.Invoke(new Highscore { Score = _score });
    }
    
    private void gamePassed()
    {
        gameOver(GameOverType.Victory);
    }

    private void destroyWorld()
    {
        //create explosions
        int numOfExplosions = 4;
        for (int i = 0; i < numOfExplosions; i++)
        {
            Instantiate(GameAssets.Instance.GlobalDestructionPS, Utilities.GetRandomWorldPositionFromScreen(), Quaternion.identity, null);
            _audioManager?.PlaySFXClip(SFXClipType.Explosion);
        }

        //shake camera
        OnWorldDestruction?.Invoke();
    }

    private float calculateFinalScore()
    {
        TotalScore totalScore = new TotalScore
        {
            Level = LevelManager.GrabLevel(),
            Accuracy = HitManager.GrabPlayerAccuracy(),
            Score = _score,
            TimeRemaining = TimeManager.GrabTimerValue()
        };

        float finalScore = totalScore.Level * (totalScore.Score + totalScore.Accuracy);
        if (totalScore.TimeRemaining > 0.0f)
            finalScore += totalScore.Level * totalScore.TimeRemaining;

        return finalScore;
    }
    
    public void EvaluateNewHighscore(float highscore)
    {
        if (_currentHighscore.Score < highscore)
        {
            _currentHighscore.Score = highscore;

            string dateString;
            if (highscore == 0.0f)
                dateString = "On 1 January 2000";
            else
            {
                System.DateTime dateTime = System.DateTime.Now;
                dateString = "On " + dateTime.Day + " " + Utilities.GetMonthName(dateTime.Month) + " " + dateTime.Year;
            }
            _currentHighscore.Date = dateString;
        }

        OnGameOverSendFinalScore?.Invoke(_currentHighscore);
    }

    public void LoadCurrentHighscore(Highscore highscore)
    {
        _currentHighscore.Score = highscore.Score;
        _currentHighscore.Date = highscore.Date;
    }

    public Highscore GetCurrentHighscore()
    {
        return _currentHighscore;
    }

    public void resetCurrentHighscore()
    {
        _currentHighscore = new Highscore();
    }

    private void affiliationChangedThisLevel()
    {
        _affiliationChangedThisLevel = true;
    }

    private void resetAffiliationChanged()
    {
        _affiliationChangedThisLevel = false;
    }

    public void QuitToMainMenu()
    {
        PauseGame(false);

        float finalScore = calculateFinalScore();
        EvaluateNewHighscore(finalScore);

        OnQuitToMainMenu?.Invoke();
    }

    public void PauseGame(bool value)
    {
        _gamePaused = value;

        Time.timeScale = _gamePaused ? 0.0f : 1.0f;
        OnGamePaused?.Invoke(value);
    }
}
