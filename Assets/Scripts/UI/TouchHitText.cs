using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TouchHitText : MonoBehaviour
{
    private TextMeshProUGUI _playerTouchesText;
    private TextMeshProUGUI _playerHits;

    private HitManager _hitManager;

    private void Awake()
    {
        _playerTouchesText = transform.Find("PlayerTouchesText").GetComponent<TextMeshProUGUI>();
        _playerHits = transform.Find("PlayerHitsText").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        HitManager.Instance.OnHitTest += updateUI;
    }

    private void OnDestroy()
    {
        HitManager.Instance.OnHitTest -= updateUI;
    }

    private void updateUI(int touches, int hits)
    {
        _playerTouchesText.SetText("Touches: " + touches.ToString());
        _playerHits.SetText("Hits: " + hits.ToString());
    }
}
