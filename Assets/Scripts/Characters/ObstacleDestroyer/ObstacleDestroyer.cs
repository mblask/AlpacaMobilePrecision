using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObstacleDestroyer : Character, ISpecialCharacter
{
    public static Action<Vector3> OnObstacleDestroyerAreaHit;

    private void Start()
    {
        AssignCharacterType(CharacterType.ObstacleDestroyer);
    }

    public void TriggerSpecialCharacter()
    {

    }

    public override void DamageThis()
    {
        base.DamageThis();

        Instantiate(GameAssets.Instance.DestructionArea, transform.position, Quaternion.identity, null);
    }
}
