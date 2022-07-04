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

    private void Awake()
    {
        AffiliationTrigger.OnAffiliationTriggerHit += switchAffiliation;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _characterType = AssignRandomCharacterType();
        _spriteRenderer.color = getCharacterColor(_characterType);
    }

    private void OnDestroy()
    {
        AffiliationTrigger.OnAffiliationTriggerHit -= switchAffiliation;
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
                return Color.red;
            case CharacterType.Negative:
                return Color.green;
            case CharacterType.Neutral:
                return Color.yellow;
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