using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AffiliationTrigger : Character, IDamagable
{
    public static Action OnAffiliationTriggerHit;

    private void Start()
    {
        AssignCharacterType(CharacterType.Neutral);
    }

    public void TriggerAffiliationSwitch()
    {
        OnAffiliationTriggerHit?.Invoke();
    }
}
