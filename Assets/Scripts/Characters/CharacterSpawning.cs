using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawning : MonoBehaviour
{
    private GameAssets _gameAssets;

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
    }

    public void SpawnNewCharacter(CharacterType characterType)
    {
        Transform newCharacterTransform = Instantiate(_gameAssets.CharacterObject, transform.position, Quaternion.identity, null);

        CharacterAnimation animation = GetComponent<CharacterAnimation>();
        animation.TestAnimation();
        CharacterGrowth growth = newCharacterTransform.GetComponent<CharacterGrowth>();
        growth.SetCharacterScale(0.3f);

        CharacterMovement movement = newCharacterTransform.GetComponent<CharacterMovement>();
        movement.ActivateRotation();
    }
}
