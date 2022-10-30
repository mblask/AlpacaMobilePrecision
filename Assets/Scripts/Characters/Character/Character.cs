using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;
using System;

public enum CharacterType
{
    Positive,
    Negative,
    AffiliationTrigger,
    ObstacleDestroyer,
}

public class Character : MonoBehaviour, IDamagable
{
    public static Action<Character> OnCharacterDestroyed;
    public static Action<PSProperties> OnParticleSystemToSpawn;

    [SerializeField] private CharacterType _characterType;
    private SpriteRenderer _spriteRenderer;

    private AudioManager _audioManager;

    private Color _positiveColor = Color.red;
    private Color _negativeColor = Color.green;
    private Color _affiliationTriggerColor = Color.yellow;
    private Color _obstacleDestroyerColor = new Color(1.0f, 0.0f, 0.7f);

    private ICharacterMove _characterMove;
    private ISpawnCharacters _characterSpawning;

    private bool _affiliationSwitched;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.sprite = GameAssets.Instance.CharacterSprites.GetRandomElement();

        _characterMove = GetComponent<ICharacterMove>();
        _characterSpawning = GetComponent<ISpawnCharacters>();

        characterInitialSetup();
    }

    private void OnEnable()
    {
        AffiliationTrigger.OnAffiliationTriggerHit += switchAffiliation;
        LevelManager.Instance.OnCharacterLevelUp += levelUpCharacter;
    }

    private void Start()
    {
        _audioManager = AudioManager.Instance;
    }

    private void OnDestroy()
    {
        AffiliationTrigger.OnAffiliationTriggerHit -= switchAffiliation;
        LevelManager.Instance.OnCharacterLevelUp -= levelUpCharacter;
    }

    private void characterInitialSetup()
    {
        _characterType = AssignRandomCharacterType();
        _spriteRenderer.color = assignCharacterColor(_characterType);
        _spriteRenderer.sprite = GameAssets.Instance.CharacterSprites.GetRandomElement();

        float zRotation = UnityEngine.Random.Range(0.0f, 180.0f);
        transform.Rotate(0.0f, 0.0f, zRotation);
    }

    private void levelUpCharacter(CharacterLevelUpProperties properties)
    {
        if (properties == null)
            return;

        _characterMove?.SetCharacterSpeedPerc(properties.PercentageSpeedIncrease);
        _characterMove?.SetDistanceDependance(properties.SpeedDistanceDependance);

        _characterSpawning?.ActivateSpawning(properties.CharactersSpawnNewCharacters);
    }

    public virtual void DamageThis()
    {
        OnParticleSystemToSpawn?.Invoke(new PSProperties{ PSposition = transform.position, PSType = PSType.Destroy, PSColor = GetCharacterColor(), PSTextureType = ParticleSystemSpawner.GetPSTextureTypeByCharacter(_characterType) });
        OnCharacterDestroyed?.Invoke(this);
        playAudioClip(SFXClipType.CharacterKilled);
        Destroy(gameObject);
    }

    private void playAudioClip(SFXClipType SFXClipType)
    {
        if (_audioManager == null)
            _audioManager = AudioManager.Instance;

        _audioManager?.PlaySFXClip(SFXClipType);
    }

    private Color assignCharacterColor(CharacterType type)
    {
        switch (type)
        {
            case CharacterType.Positive:
                return _positiveColor;
            case CharacterType.Negative:
                return _negativeColor;
            case CharacterType.AffiliationTrigger:
                return _affiliationTriggerColor;
            case CharacterType.ObstacleDestroyer:
                return _obstacleDestroyerColor;
            default:
                return Color.black;
        }
    }

    public Color GetCharacterColor()
    {
        return _spriteRenderer.color;
    }

    public CharacterType AssignRandomCharacterType()
    {
        if (Utilities.ChanceFunc(50))
            return CharacterType.Positive;
        else
            return CharacterType.Negative;
    }

    public void AssignCharacterType(CharacterType type)
    {
        _characterType = type;
        _spriteRenderer.color = assignCharacterColor(_characterType);

        switch (_characterType)
        {
            case CharacterType.Positive:
            case CharacterType.Negative:
                _spriteRenderer.sprite = GameAssets.Instance.CharacterSprites.GetRandomElement();
                break;
            case CharacterType.AffiliationTrigger:
                _spriteRenderer.sprite = GameAssets.Instance.AffiliationTriggerSprites.GetRandomElement();
                break;
            case CharacterType.ObstacleDestroyer:
                _spriteRenderer.sprite = GameAssets.Instance.ObstacleDestroyerSprites.GetRandomElement();
                break;
            default:
                break;
        }
    }

    public CharacterType GetCharacterType()
    {
        return _characterType;
    }

    private void switchAffiliation()
    {
        _affiliationSwitched = true;

        switch (_characterType)
        {
            case CharacterType.Positive:
                _characterType = CharacterType.Negative;
                break;
            case CharacterType.Negative:
                _characterType = CharacterType.Positive;
                break;
            default:
                break;
        }
    }

    public void RedoAffiliation()
    {
        switchAffiliation();
    }

    public bool IsAffiliationSwitched()
    {
        return _affiliationSwitched;
    }
}