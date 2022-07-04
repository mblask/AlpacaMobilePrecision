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
        GameManager.Instance.OnGameOver += activateGameOverScreen;
    }

    private void OnDestroy()
    {

        GameManager.Instance.OnGameOver -= activateGameOverScreen;
    }

    private void activateGameOverScreen()
    {
        _gameOverContainer.gameObject.SetActive(true);
    }
}
