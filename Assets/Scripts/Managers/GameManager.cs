using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FinalScore
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
    public event Action<FinalScore> OnGameOverSendFinalScore;
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
        TimeManager.Instance.OnTimeIsOut += gameOver;

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
        TimeManager.Instance.OnTimeIsOut -= gameOver;
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
                    gameOver();
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

    private void gameOver()
    {
        Debug.LogError("Game Over");
        Debug.Log("Activate game over screen and disable the touch feature.");
        OnGameOver?.Invoke();
    }

    private void destroyWorld()
    {
        //create explosions
        int numOfExplosions = 4;
        for (int i = 0; i < numOfExplosions; i++)
        {
            Instantiate(GameAssets.Instance.GlobalDestructionPS, AlpacaUtils.GetRandomWorldPosition(), Quaternion.identity, null);
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
        ActivatePauseGame();
        OnQuitToMainMenu?.Invoke();
    }

    public void ActivatePauseGame()
    {
        _gamePaused = !_gamePaused;

        Time.timeScale = _gamePaused ? 0.0f : 1.0f;
        OnGamePaused?.Invoke(_gamePaused);
    }
}
