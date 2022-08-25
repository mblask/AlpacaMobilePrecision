using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum CharacterType
{
    Positive,
    Negative,
    Neutral,
}

public class Character : MonoBehaviour, IDamagable
{
    public static Action<Character> OnCharacterDestroyed;
    public static Action<PSProperties> OnParticleSystemToSpawn;

    [SerializeField] private CharacterType _characterType;
    private SpriteRenderer _spriteRenderer;

    private Color _positiveColor = Color.red;
    private Color _negativeColor = Color.green;
    private Color _neutralColor = Color.yellow;

    private ICharacterMove _characterMove;
    private ISpawnCharacters _characterSpawning;

    private void Awake()
    {
        AffiliationTrigger.OnAffiliationTriggerHit += switchAffiliation;
        LevelManager.Instance.OnCharacterLevelUp += levelUpCharacter;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _characterType = AssignRandomCharacterType();
        _spriteRenderer.color = getCharacterColor(_characterType);

        _characterMove = GetComponent<ICharacterMove>();
        _characterSpawning = GetComponent<ISpawnCharacters>();
    }

    private void OnDestroy()
    {
        AffiliationTrigger.OnAffiliationTriggerHit -= switchAffiliation;
        LevelManager.Instance.OnCharacterLevelUp -= levelUpCharacter;
    }

    private void levelUpCharacter(CharacterLevelUpProperties properties)
    {
        _characterMove?.SetCharacterSpeedPerc(properties.PercentageSpeedIncrease);
        _characterMove?.SetDistanceDependance(properties.SpeedDistanceDependance);

        _characterSpawning?.ActivateSpawning(properties.CharactersSpawnNewCharacters);
    }

    public void DamageThis()
    {
        OnParticleSystemToSpawn?.Invoke(new PSProperties{ PSposition = transform.position, PSType = PSType.Destroy, PSColor = _spriteRenderer.color });
        OnCharacterDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    private Color getCharacterColor(CharacterType type)
    {
        switch (type)
        {
            case CharacterType.Positive:
                return _positiveColor;
            case CharacterType.Negative:
                return _negativeColor;
            case CharacterType.Neutral:
                return _neutralColor;
            default:
                return Color.black;
        }
    }

    public CharacterType AssignRandomCharacterType()
    {
        if (AlpacaUtils.ChanceFunc(50))
            return CharacterType.Positive;
        else
            return CharacterType.Negative;
    }

    public void AssignCharacterType(CharacterType type)
    {
        _characterType = type;
        _spriteRenderer.color = getCharacterColor(_characterType);
    }

    public CharacterType GetCharacterType()
    {
        return _characterType;
    }

    private void switchAffiliation()
    {
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
}