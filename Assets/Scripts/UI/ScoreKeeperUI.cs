using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreKeeperUI : MonoBehaviour
{
    private TextMeshProUGUI _scoreText;

    private void Awake()
    {
        GameManager.Instance.OnScoreUpdate += UpdateScoreText;
        
        _scoreText = transform.Find("Score").GetComponent<TextMeshProUGUI>();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnScoreUpdate -= UpdateScoreText;
    }

    public void UpdateScoreText(int score)
    {
        _scoreText.SetText("Score: " + score.ToString());
    }
}
