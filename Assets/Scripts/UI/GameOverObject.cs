using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverObject : MonoBehaviour
{
    private Transform _gameOverContainer;

    private void Awake()
    {
        _gameOverContainer = transform.Find("Container");
    }

    private void Start()
    {
        deactivateGameOverScreen();
        GameManager.Instance.OnGameOver += activateGameOverScreen;
        LevelManager.Instance.OnQuitToMainMenu += deactivateGameOverScreen;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameOver -= activateGameOverScreen;
        LevelManager.Instance.OnQuitToMainMenu += deactivateGameOverScreen;
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
