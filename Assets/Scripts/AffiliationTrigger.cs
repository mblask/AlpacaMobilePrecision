using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AffiliationTrigger : Character, ISpecialCharacter
{
    public static Action OnAffiliationTriggerHit;

    private void Start()
    {
        AssignCharacterType(CharacterType.AffiliationTrigger);
    }

    public void TriggerSpecialCharacter()
    {
        OnAffiliationTriggerHit?.Invoke();
    }
}
