using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AlpacaMyGames;

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
        //_spriteRenderer.sprite = GameAssets.Instance.ObstacleSprites.GetRandomElement();
        _obstacleAnimation = GetComponent<ObjectAnimation>();
    }

    private void Start()
    {
        _obstacleType = getObstacleType();
        _spriteRenderer.sprite = getObstacleSprite(_obstacleType);
        _spriteRenderer.color = getObstacleColor(_obstacleType);
        _obstacleHitPoint = getObstacleHitPoints(_obstacleType);

        _audioManager = AudioManager.Instance;
    }

    public void DamageThis()
    {
        _obstacleHitPoint--;
        _obstacleAnimation.PlayAnimation(AnimationType.ContractRelease);
        _audioManager?.PlaySFXClip(SFXClipType.ObstacleHit);

        if (_obstacleHitPoint <= 0 && _obstacleType.Equals(ObstacleType.Fragile))
            DestroyObstacle();
    }

    public void DestroyObstacle()
    {
        OnObstacleDestroy?.Invoke(this);
        OnParticleSystemToSpawn?.Invoke(new PSProperties { PSposition = transform.position, PSType = PSType.Destroy, PSColor = getObstacleColor(_obstacleType), PSTextureType = PSTextureType.Obstacle });
        _audioManager?.PlaySFXClip(SFXClipType.ObstacleSmashed);
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
                return new Color(0.8f, 0.8f, 0.8f);
            case ObstacleType.Fragile:
                return new Color(0.1f, 0.5f, 0.8f);
            default:
                return Color.black;
        }
    }

    private Sprite getObstacleSprite(ObstacleType type)
    {
        switch (type)
        {
            case ObstacleType.Solid:
                return GameAssets.Instance.IndestructableObstacleSprites.GetRandomElement();
            case ObstacleType.Fragile:
                return GameAssets.Instance.ObstacleSprites.GetRandomElement();
            default:
                return null;
        }
    }

    private ObstacleType getObstacleType()
    {
        int numOfObstacles = System.Enum.GetValues(typeof(ObstacleType)).Length;

        return (ObstacleType)UnityEngine.Random.Range(0, numOfObstacles);
    }
}