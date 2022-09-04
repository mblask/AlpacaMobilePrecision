using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HitManager : MonoBehaviour
{
    public Action<float> OnSendPlayerAccuracy;

    private static HitManager _instance;

    public static HitManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private GameAssets _gameAssets;

    [Header("Hit effect")]
    [SerializeField][Range(0.0f, 2.0f)] private float _nearbyHitRadius = 1.0f;
    [SerializeField] private float _obstacleDestroyerRadius = 3.0f;

    private List<Transform> _bulletMarksList = new List<Transform>();

    private int _playerTouchNumber = 0;
    private int _playerHit = 0;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        PlayerTouchManager.Instance.OnPlayerTouchPosition += playerTouchManager_OnPlayerTouch;
        LevelManager.Instance.OnLoadLevel += levelManager_onLoadLevel;
        ObstacleDestroyer.OnObstacleDestroyerAreaHit += destroyObstaclesInArea;

        _gameAssets = GameAssets.Instance;
    }

    private void OnDestroy()
    {
        PlayerTouchManager.Instance.OnPlayerTouchPosition -= playerTouchManager_OnPlayerTouch;
        LevelManager.Instance.OnLoadLevel += levelManager_onLoadLevel;
        ObstacleDestroyer.OnObstacleDestroyerAreaHit -= destroyObstaclesInArea;
    }

    private void levelManager_onLoadLevel(int levelNumber)
    {
        resetAccuracyCount();
        clearBulletMarks();
    }

    private void playerTouchManager_OnPlayerTouch(Vector2 worldPosition)
    {
        _playerTouchNumber++;

        _bulletMarksList.Add(Instantiate(_gameAssets.BulletMark, worldPosition, Quaternion.identity, null));
        detectCharacterHit(worldPosition);
        detectAreaEffectHits(worldPosition);
        OnSendPlayerAccuracy?.Invoke(GetPlayerAccuracy());
    }

    private void destroyObstaclesInArea(Vector3 worldPosition)
    {
        Collider2D[] hitInfo = Physics2D.OverlapCircleAll(worldPosition, _obstacleDestroyerRadius);

        if (hitInfo.Length != 0)
        {
            foreach (Collider2D collider in hitInfo)
            {
                Obstacle obstacle = collider.GetComponent<Obstacle>();
                if (obstacle != null)
                    obstacle.DestroyObstacle();
            }
        }
    }

    private void detectAreaEffectHits(Vector3 worldPosition)
    {
        Collider2D[] hitInfo = Physics2D.OverlapCircleAll(worldPosition, _nearbyHitRadius);

        if (hitInfo.Length != 0)
        {
            foreach (Collider2D hit in hitInfo)
            {
                ICharacterMove characterMovement = hit.GetComponent<ICharacterMove>();

                if (characterMovement != null)
                {
                    characterMovement.ActivateRotation();
                    characterMovement.NearbyHitDetectedAt(worldPosition);
                }

                ICharacterAnimation characterAnimation = hit.GetComponent<ICharacterAnimation>();

                if (characterAnimation != null)
                    characterAnimation.PlayAnimation(AnimationType.ContractRelease);
            }
        }
    }

    private void detectCharacterHit(Vector2 worldPosition)
    {
        RaycastHit2D[] hitInfoArray = Physics2D.RaycastAll(worldPosition, Vector2.up, 0.1f);
        if (hitInfoArray.Length >= 1)
        {
            bool obstacleExists = false;

            foreach (RaycastHit2D hit in hitInfoArray)
            {
                Obstacle obstacle = hit.collider.GetComponent<Obstacle>();
                if (obstacle != null)
                {
                    obstacleExists = true;
                    obstacle.DamageThis();
                    break;
                }
            }

            if (!obstacleExists)
            {
                Collider2D firstCollider = hitInfoArray[0].collider;

                IDamagable damagable = firstCollider.GetComponent<IDamagable>();

                if (damagable != null)
                {
                    ISpecialCharacter specialCharacter = firstCollider.GetComponent<ISpecialCharacter>();
                    if (specialCharacter != null)
                        specialCharacter.TriggerSpecialCharacter();

                    Character character = firstCollider.GetComponent<Character>();
                    if (character != null && character.GetCharacterType().Equals(CharacterType.Negative))
                        _playerHit++;

                    damagable.DamageThis();
                }
            }
        }
    }

    private void clearBulletMarks()
    {
        foreach (Transform bulletMark in _bulletMarksList)
        {
            if (bulletMark != null)
                Destroy(bulletMark.gameObject);
        }
        _bulletMarksList.Clear();
    }

    public static float GrabPlayerAccuracy()
    {
        return _instance.GetPlayerAccuracy();
    }

    public float GetPlayerAccuracy()
    {
        if (_playerTouchNumber == 0)
            return 0.0f;

        return (float)_playerHit / _playerTouchNumber;
    }

    private void resetAccuracyCount()
    {
        _playerHit = 0;
        _playerTouchNumber = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
    }
}
