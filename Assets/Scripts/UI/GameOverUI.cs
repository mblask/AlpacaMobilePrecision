using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AlpacaMyGames;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private bool _randomGameOverText;

    private Transform _gameOverContainer;
    private TextMeshProUGUI _gameOverText;
    private TextMeshProUGUI _finalScoreText;

    [TextArea] [SerializeField] private List<string> _gameOverMessages;
    [TextArea] [SerializeField] private List<string> _gameOverOnTimeMessages;
    [TextArea] [SerializeField] private List<string> _gameVictoryMessages;

    private void Awake()
    {
        _gameOverContainer = transform.Find("Container");
        _gameOverText = _gameOverContainer.Find("GameOverText").GetComponent<TextMeshProUGUI>();
        _finalScoreText = _gameOverContainer.Find("FinalScoreText").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        deactivateGameOverScreen();

        GameManager.Instance.OnGameOver += activateGameOverScreen;
        GameManager.Instance.OnGameOverOnTime += gameOverOnTime;
        GameManager.Instance.OnGameOverOnScore += gameOverOnScore;
        GameManager.Instance.OnQuitToMainMenu += deactivateGameOverScreen;
        GameManager.Instance.OnGameOverSendFinalScore += updateHighsoreValue;
        GameManager.Instance.OnWorldDestruction += changeGameOverTextOnWorldDestruction;
        LevelManager.Instance.OnGamePassed += changeGameOverTextOnVictory;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameOver -= activateGameOverScreen;
        GameManager.Instance.OnGameOverOnTime -= gameOverOnTime;
        GameManager.Instance.OnGameOverOnScore -= gameOverOnScore;
        GameManager.Instance.OnQuitToMainMenu -= deactivateGameOverScreen;
        GameManager.Instance.OnGameOverSendFinalScore -= updateHighsoreValue;
        GameManager.Instance.OnWorldDestruction -= changeGameOverTextOnWorldDestruction;
        LevelManager.Instance.OnGamePassed -= changeGameOverTextOnVictory;
    }

    private void updateHighsoreValue(Highscore highscore)
    {
        updateFinalScore(highscore.Score);
    }

    private void gameOverOnTime()
    {
        _gameOverText.SetText(_gameOverOnTimeMessages.GetRandomElement());
    }

    private void gameOverOnScore()
    {
        _gameOverText.SetText("Lost on score, you say?");
    }

    private void changeGameOverTextOnWorldDestruction()
    {
        if (_randomGameOverText)
        {
            setTextColor(Color.red);
            _gameOverText.SetText(_gameOverMessages.GetRandomElement());
        }
    }

    private void changeGameOverTextOnVictory()
    {
        if (_randomGameOverText)
        {
            setTextColor(Color.cyan);
            _gameOverText.SetText(_gameVictoryMessages.GetRandomElement());
        }
    }

    private void setTextColor(Color color)
    {
        _gameOverText.color = color;
        _finalScoreText.color = color;
    }

    private void activateGameOverScreen()
    {
        _gameOverContainer.gameObject.SetActive(true);
    }

    private void deactivateGameOverScreen()
    {
        _gameOverContainer.gameObject.SetActive(false);
    }

    private void updateFinalScore(float score)
    {
        _finalScoreText.SetText("Score: " + score.ToString("F0"));
    }
}
