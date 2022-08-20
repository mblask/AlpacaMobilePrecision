using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private bool _randomGameOverText;

    private Transform _gameOverContainer;
    private TextMeshProUGUI _gameOverText;
    private TextMeshProUGUI _finalScoreText;

    [TextArea] [SerializeField] private List<string> _gameOverMessages;
    [TextArea] [SerializeField] private List<string> _gameOverOnTimeMessages;

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
        GameManager.Instance.OnGameOverSendFinalScore += updateFinalScore;
        GameManager.Instance.OnWorldDestruction += changeGameOverTextOnWorldDestruction;
        GameManager.Instance.OnQuitToMainMenu += deactivateGameOverScreen;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameOver -= activateGameOverScreen;
        GameManager.Instance.OnGameOverOnTime += gameOverOnTime;
        GameManager.Instance.OnGameOverSendFinalScore -= updateFinalScore;
        GameManager.Instance.OnWorldDestruction -= changeGameOverTextOnWorldDestruction;
        GameManager.Instance.OnQuitToMainMenu -= deactivateGameOverScreen;
    }

    private void gameOverOnTime()
    {
        _gameOverText.SetText(_gameOverOnTimeMessages[Random.Range(0, _gameOverOnTimeMessages.Count)]);
    }

    private void changeGameOverTextOnWorldDestruction()
    {
        if (_randomGameOverText)
            _gameOverText.SetText(_gameOverMessages[Random.Range(0, _gameOverMessages.Count)]);
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
