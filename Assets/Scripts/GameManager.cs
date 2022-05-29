using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public event Action<int> OnScoreUpdate;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [Header("Score Points")]
    [SerializeField] private int _score = 0;

    [Header("Points On Destroy")]
    [SerializeField] private int _posCharHit = -10;
    [SerializeField] private int _negCharHit = 15;
    [SerializeField] private int _fragileObstHit = -2;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        Character.OnCharacterDestroyed += updateScoreByCharacterType;
        Obstacle.OnObstacleDestroy += updateScoreByObstacle;

        ResetScore();

        if (Screen.orientation != ScreenOrientation.LandscapeRight)
            Screen.orientation = ScreenOrientation.LandscapeRight;
    }

    private void OnDestroy()
    {
        Character.OnCharacterDestroyed -= updateScoreByCharacterType;
        Obstacle.OnObstacleDestroy -= updateScoreByObstacle;
    }

    private void updateScoreByObstacle(Obstacle obstacle)
    {
        UpdateScore(_fragileObstHit);
    }

    private void updateScoreByCharacterType(Character character)
    {
        switch (character.GetCharacterType())
        {
            case CharacterType.Positive:
                UpdateScore(_posCharHit);
                break;
            case CharacterType.Negative:
                UpdateScore(_negCharHit);
                break;
            default:
                UpdateScore(0);
                break;
        }
    }

    public void UpdateScore(int score)
    {
        _score += score;
        OnScoreUpdate?.Invoke(_score);
    }

    public void ResetScore()
    {
        _score = 0;
        OnScoreUpdate?.Invoke(_score);
    }
}
