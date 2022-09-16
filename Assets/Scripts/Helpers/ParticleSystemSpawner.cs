using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public enum PSType
{
    Destroy,
}

public class ParticleSystemSpawner : MonoBehaviour
{
    private static ParticleSystemSpawner _instance;

    private GameAssets _gameAssets;

    private void Awake()
    {
        _instance = this;
    }

    private void OnEnable()
    {
        Character.OnParticleSystemToSpawn += SpawnParticleSystem;
        Obstacle.OnParticleSystemToSpawn += SpawnParticleSystem;
        AffiliationTrigger.OnParticleSystemToSpawn += SpawnParticleSystem;
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
    }

    private void OnDestroy()
    {
        Character.OnParticleSystemToSpawn -= SpawnParticleSystem;
        Obstacle.OnParticleSystemToSpawn -= SpawnParticleSystem;
        AffiliationTrigger.OnParticleSystemToSpawn -= SpawnParticleSystem;
    }

    public void SpawnParticleSystem(PSProperties properties)
    {
        Transform PSTransform = Instantiate(getPSPrefab(properties.PSType), properties.PSposition, Quaternion.identity, null);
        if (PSTransform != null)
        {
            ParticleSystem particleSystem = PSTransform.GetComponent<ParticleSystem>();
            particleSystem.textureSheetAnimation.AddSprite(getPSTexture(properties.PSTextureType));

            ParticleSystem.MainModule destroyPSMainModule = particleSystem.main;
            destroyPSMainModule.startColor = properties.PSColor;
            particleSystem.Play();
        }
    }

    private Transform getPSPrefab(PSType type)
    {
        switch (type)
        {
            case PSType.Destroy:
                return GameAssets.Instance.DestroyObjectPS;
            default:
                return null;
        }
    }

    private Sprite getPSTexture(PSTextureType psTextureType)
    {
        switch (psTextureType)
        {
            case PSTextureType.Character:
                return _gameAssets.CharacterSprites.GetRandomElement();
            case PSTextureType.Obstacle:
                return _gameAssets.ObstacleDestroyerSprites.GetRandomElement();
            case PSTextureType.AffTrigger:
                return _gameAssets.AffiliationTriggerSprites.GetRandomElement();
            case PSTextureType.ObstDestroyer:
                return _gameAssets.ObstacleDestroyerSprites.GetRandomElement();
            default:
                return null;
        }
    }

    private PSTextureType getPSTextureTypeByCharacter(CharacterType characterType)
    {
        switch (characterType)
        {
            case CharacterType.Positive:
            case CharacterType.Negative:
                return PSTextureType.Character;
            case CharacterType.AffiliationTrigger:
                return PSTextureType.AffTrigger;
            case CharacterType.ObstacleDestroyer:
                return PSTextureType.ObstDestroyer;
            default:
                return PSTextureType.Character;
        }
    }

    public static PSTextureType GetPSTextureTypeByCharacter(CharacterType characterType)
    {
        return _instance.getPSTextureTypeByCharacter(characterType);
    }
}
