using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ObstacleType
{
    Solid,
    Fragile,
}

public class Obstacle : MonoBehaviour, IDamagable
{
    public static Action<Obstacle> OnObstacleDestroy;
    public static Action<PSProperties> OnParticleSystemToSpawn;

    private AudioManager _audioManager;

    private int _solidHitPoint = 9999;

    private ObstacleType _obstacleType;
    private SpriteRenderer _spriteRenderer;

    private ObjectAnimation _obstacleAnimation;

    private int _obstacleHitPoint;

    private void Awake()
    {
        _spriteRenderer = transform.Find("ObstacleBody").GetComponent<SpriteRenderer>();
        _obstacleAnimation = GetComponent<ObjectAnimation>();
    }

    private void Start()
    {
        _obstacleType = getObstacleType();
        _spriteRenderer.color = getObstacleColor(_obstacleType);
        _obstacleHitPoint = getObstacleHitPoints(_obstacleType);

        _audioManager = AudioManager.Instance;
    }

    public void DamageThis()
    {
        _obstacleHitPoint--;
        _obstacleAnimation.PlayAnimation(AnimationType.ContractRelease);
        _audioManager?.PlaySFXClip(AudioType.ObstacleHit);

        if (_obstacleHitPoint <= 0 && _obstacleType.Equals(ObstacleType.Fragile))
            DestroyObstacle();
    }

    public void DestroyObstacle()
    {
        OnObstacleDestroy?.Invoke(this);
        OnParticleSystemToSpawn?.Invoke(new PSProperties { PSposition = transform.position, PSType = PSType.Destroy, PSColor = getObstacleColor(_obstacleType) });
        _audioManager?.PlaySFXClip(AudioType.ObstacleSmashed);
        Destroy(gameObject);
    }

    private int getObstacleHitPoints(ObstacleType type)
    {
        switch (type)
        {
            case ObstacleType.Solid:
                return _solidHitPoint;
            case ObstacleType.Fragile:
                return UnityEngine.Random.Range(1, 3);
            default:
                return 0;
        }
    }

    private Color getObstacleColor(ObstacleType type)
    {
        switch (type)
        {
            case ObstacleType.Solid:
                return Color.gray;
            case ObstacleType.Fragile:
                return Color.blue;
            default:
                return Color.black;
        }
    }

    private ObstacleType getObstacleType()
    {
        int numOfObstacles = System.Enum.GetValues(typeof(ObstacleType)).Length;

        return (ObstacleType)UnityEngine.Random.Range(0, numOfObstacles);
    }
}