using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AffiliationTrigger : Character, ISpecialCharacter
{
    public static Action OnAffiliationTriggerHit;

    [Header("Read-only")]
    [SerializeField] private bool _triggerOn = true;

    private void Start()
    {
        AssignCharacterType(CharacterType.AffiliationTrigger);
    }

    public void TriggerSpecialCharacter()
    {
        if (_triggerOn)
            OnAffiliationTriggerHit?.Invoke();
    }

    public void SetTriggerOn(bool value)
    {
        _triggerOn = value;
    }
}
