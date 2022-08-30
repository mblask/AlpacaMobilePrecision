using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreKeeperUI : MonoBehaviour
{
    private TextMeshProUGUI _scoreText;
    private Animator _animator;
    private string _pointsScoredTrigger = "PointsScored";

    private void Awake()
    {
        GameManager.Instance.OnScoreUpdate += UpdateScoreText;
        
        _scoreText = transform.Find("ScoreNumber").GetComponent<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnScoreUpdate -= UpdateScoreText;
    }

    public void UpdateScoreText(int score)
    {
        _scoreText.SetText(score.ToString());
        _animator.SetTrigger(_pointsScoredTrigger);
    }
}
