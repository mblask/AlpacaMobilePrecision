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
        _scoreText = transform.Find("ScoreNumber").GetComponent<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.OnScoreUpdate += UpdateScoreText;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnScoreUpdate -= UpdateScoreText;
    }

    public void UpdateScoreText(int score)
    {
        _scoreText.SetText(score.ToString());
        _animator.SetTrigger(_pointsScoredTrigger);
    }
}
