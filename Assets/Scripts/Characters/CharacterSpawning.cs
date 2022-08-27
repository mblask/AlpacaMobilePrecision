using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterSpawning : MonoBehaviour, ISpawnCharacters
{
    public static Action<Transform> OnCharacterSpawn;

    private GameAssets _gameAssets;
    private Character _character;

    private float _spawnTimer;
    private float _minSpawnTime = 5.0f;
    private float _maxSpawnTime = 10.0f;

    [SerializeField] private bool _isSpawning = false;

    private void Awake()
    {
        _character = GetComponent<Character>();
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
        _spawnTimer = UnityEngine.Random.Range(_minSpawnTime, _maxSpawnTime);
    }

    private void Update()
    {
        spawnProcedure();
    }

    private void spawnProcedure()
    {
        if (!_isSpawning)
            return;

        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0.0f)
        {
            if (AlpacaUtils.ChanceFunc(10))
                SpawnNewCharacter(_character.GetCharacterType());

            _spawnTimer = UnityEngine.Random.Range(_minSpawnTime, _maxSpawnTime);
        }
    }

    public void SpawnNewCharacter(CharacterType characterType)
    {
        Transform newCharacterTransform = Instantiate(_gameAssets.CharacterObject, transform.position, Quaternion.identity, null);

        OnCharacterSpawn?.Invoke(newCharacterTransform);

        CharacterAnimation animation = newCharacterTransform.GetComponent<CharacterAnimation>();
        animation.PlayAnimation(AnimationType.ContractRelease);

        CharacterGrowth growth = newCharacterTransform.GetComponent<CharacterGrowth>();
        growth.SetCharacterScale(0.3f);

        CharacterMovement movement = newCharacterTransform.GetComponent<CharacterMovement>();
        movement.ActivateRotation();
    }

    public void ActivateSpawning(bool value)
    {
        _isSpawning = value;
    }
}