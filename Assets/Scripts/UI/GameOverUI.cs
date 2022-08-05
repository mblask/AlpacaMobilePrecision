using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private bool _randomGameOverText;

    private Transform _gameOverContainer;
    private TextMeshProUGUI _gameOverText;

    [TextArea]
    [SerializeField] private List<string> _gameOverMessages;

    private void Awake()
    {
        _gameOverContainer = transform.Find("Container");
        _gameOverText = _gameOverContainer.Find("GameOverText").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        deactivateGameOverScreen();
        GameManager.Instance.OnGameOver += activateGameOverScreen;
        GameManager.Instance.OnWorldDestruction += changeGameOverTextOnWorldDestruction;
        LevelManager.Instance.OnQuitToMainMenu += deactivateGameOverScreen;
    }


    private void OnDestroy()
    {
        GameManager.Instance.OnGameOver -= activateGameOverScreen;
        GameManager.Instance.OnWorldDestruction -= changeGameOverTextOnWorldDestruction;
        LevelManager.Instance.OnQuitToMainMenu -= deactivateGameOverScreen;
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
}
