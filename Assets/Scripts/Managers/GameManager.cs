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

public class TotalScore
{
    public int Score;
    public int Level;
    public float Accuracy;
    public float TimeRemaining;
}

public class GameManager : MonoBehaviour
{
    public event Action<int> OnScoreUpdate;
    public event Action OnGameOver;
    public event Action<float> OnGameOverSendFinalScore;
    public event Action OnGameOverOnTime;
    public event Action<bool> OnGamePaused;
    public event Action OnQuitToMainMenu;
    public event Action OnWorldDestruction;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [Header("Score Points")]
    [SerializeField] private int _score = 0;
    private float _currentHighscore;

    [Header("Points On Destroy")]
    [SerializeField] private int _posCharHit = -10;
    [SerializeField] private int _negCharHit = 15;
    [SerializeField] private int _fragileObstHit = -2;

    private bool _affiliationChangedThisLevel = false;

    private bool _gamePaused = false;

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
        LevelManager.Instance.OnGameRestart += resetScore;
        LevelManager.Instance.OnGamePassed += gamePassed;
        TimeManager.Instance.OnTimeIsOut += gameOverOnTime;

        resetScore();

        if (Screen.orientation != ScreenOrientation.LandscapeRight)
            Screen.orientation = ScreenOrientation.LandscapeRight;
    }

    private void OnDestroy()
    {
        Character.OnCharacterDestroyed -= updateScoreByCharacterType;
        Obstacle.OnObstacleDestroy -= updateScoreByObstacle;
        AffiliationTrigger.OnAffiliationTriggerHit -= affiliationChangedThisLevel;
        LevelManager.Instance.OnLoadLevel -= gameManager_onLoadLevel;
        LevelManager.Instance.OnGameReload -= resetScore;
        LevelManager.Instance.OnGameRestart -= resetScore;
        LevelManager.Instance.OnGamePassed -= gamePassed;
        TimeManager.Instance.OnTimeIsOut -= gameOverOnTime;
    }

    private void gameOverOnTime()
    {
        gameOver(GameOverType.Failure);
        OnGameOverOnTime?.Invoke();
    }

    private void gameManager_onLoadLevel(int level)
    {
        resetAffiliationChanged();
    }

    private void updateScoreByObstacle(Obstacle obstacle)
    {
        updateScore(_fragileObstHit);
    }

    private void updateScoreByCharacterType(Character character)
    {
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
        _score += scoreIncrement;
        OnScoreUpdate?.Invoke(_score);
    }

    private void resetScore()
    {
        _score = 0;
        OnScoreUpdate?.Invoke(_score);
    }

    private void gameOver(GameOverType gameOverType)
    {
        switch (gameOverType)
        {
            case GameOverType.Victory:
                Debug.LogError("Victory!!");
                break;
            case GameOverType.Failure:
                Debug.LogError("Game Over");
                break;
            default:
                break;
        }

        //get level number, current score, accuracy, time remaining
        TotalScore totalScore = new TotalScore { Level = LevelManager.GrabLevel(), Accuracy = HitManager.GrabPlayerAccuracy(), Score = _score, TimeRemaining = TimeManager.GrabTimerValue() };

        //calculate final score
        float finalScore = calculateFinalScore(totalScore);

        //send final score to the GameOverUI
        OnGameOver?.Invoke();
        evaluateNewHighscore(finalScore);
    }

    private void gamePassed()
    {
        gameOver(GameOverType.Victory);
    }

    private void evaluateNewHighscore(float highscore)
    {
        if (_currentHighscore < highscore)
        {
            _currentHighscore = highscore;
            OnGameOverSendFinalScore?.Invoke(highscore);
        }
    }

    public float GetCurrentHighscore()
    {
        return _currentHighscore;
    }

    private float calculateFinalScore(TotalScore totalScore)
    {
        float finalScore = totalScore.Level * (totalScore.Score + totalScore.Accuracy);
        if (totalScore.TimeRemaining > 0.0f)
            finalScore += totalScore.Level * totalScore.TimeRemaining;

        return finalScore;
    }

    private void destroyWorld()
    {
        //create explosions
        int numOfExplosions = 4;
        for (int i = 0; i < numOfExplosions; i++)
        {
            Instantiate(GameAssets.Instance.GlobalDestructionPS, Utilities.GetRandomWorldPosition(), Quaternion.identity, null);
        }

        //shake camera
        OnWorldDestruction?.Invoke();
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
        PauseGame(true);
        OnQuitToMainMenu?.Invoke();
    }

    public void PauseGame(bool value)
    {
        _gamePaused = value;

        Time.timeScale = _gamePaused ? 0.0f : 1.0f;
        OnGamePaused?.Invoke(_gamePaused);
    }
}
