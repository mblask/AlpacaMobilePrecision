using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AlpacaMyGames;

public class CharacterSpawning : MonoBehaviour, ISpawnCharacters
{
    public static Action<Transform> OnCharacterSpawn;

    private GameAssets _gameAssets;
    private Character _character;
    private CharacterMovement _characterMovement;
    private AudioManager _audioManager;

    private float _spawnTimer;
    private float _minSpawnTime = 5.0f;
    private float _maxSpawnTime = 10.0f;

    [SerializeField] private bool _isSpawning = false;

    private void Awake()
    {
        _character = GetComponent<Character>();
        _characterMovement = GetComponent<CharacterMovement>();
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
        _audioManager = AudioManager.Instance;
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
            if (Utilities.ChanceFunc(10))
                spawnNewCharacter();

            _spawnTimer = UnityEngine.Random.Range(_minSpawnTime, _maxSpawnTime);
        }
    }

    private void spawnNewCharacter()
    {
        Transform newCharacterTransform = Instantiate(_gameAssets.CharacterObject, transform.position, Quaternion.identity, null);
        
        OnCharacterSpawn?.Invoke(newCharacterTransform);

        Character spawnedCharacter = newCharacterTransform.GetComponent<Character>();
        if (_character.IsAffiliationSwitched())
            spawnedCharacter.RedoAffiliation();

        ObjectAnimation animation = newCharacterTransform.GetComponent<ObjectAnimation>();
        animation.PlayAnimation(AnimationType.ContractRelease);

        CharacterGrowth growth = newCharacterTransform.GetComponent<CharacterGrowth>();
        growth.SetCharacterScale(0.3f);

        CharacterMovement movement = newCharacterTransform.GetComponent<CharacterMovement>();
        movement.SetCharacterSpeed(_characterMovement.GetCharacterSpeed());
        movement.ActivateRotation();

        _audioManager?.PlaySFXClip(SFXClipType.Spawning);
    }

    public void ActivateSpawning(bool value)
    {
        _isSpawning = value;
    }
}