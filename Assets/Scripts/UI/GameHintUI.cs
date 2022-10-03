using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHintUI : MonoBehaviour
{
    private Transform _container;
    private Button _closeButton;

    private GameManager _gameManager;

    private void Awake()
    {
        _container = transform.Find("Container");
        _closeButton = _container.Find("CloseButton").GetComponent<Button>();

        _gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        _gameManager.OnFirstLaunch += activateUI;
    }

    private void Start()
    {
        _closeButton.onClick.AddListener(closeUI);
    }

    private void OnDestroy()
    {
        _gameManager.OnFirstLaunch -= activateUI;
    }

    private void activateUI()
    {
        _container.gameObject.SetActive(true);
    }

    private void closeUI()
    {
        _container.gameObject.SetActive(false);
    }
}
