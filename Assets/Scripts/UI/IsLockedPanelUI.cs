using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IsLockedPanelUI : MonoBehaviour
{
    [SerializeField] private Color _lockedColor;
    [SerializeField] private Color _openColor;

    private Image _panelImage;

    private void Awake()
    {
        _panelImage = GetComponent<Image>();
    }

    public void ActivatePanel(bool value)
    {
        _panelImage.color = value ? _openColor : _lockedColor;
    }
}
