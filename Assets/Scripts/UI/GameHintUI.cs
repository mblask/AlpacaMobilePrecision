using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHintUI : MonoBehaviour
{
    private Transform _container;
    private Button _closeButton;

    private void Awake()
    {
        _container = transform.Find("Container");
        _closeButton = _container.Find("CloseButton").GetComponent<Button>();
    }

    private void Start()
    {
        GameManager.Instance.OnFirstLaunch += activateUI;

        _closeButton.onClick.AddListener(closeUI);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnFirstLaunch -= activateUI;
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
