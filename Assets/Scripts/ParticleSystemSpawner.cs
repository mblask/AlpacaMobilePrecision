using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PSType
{
    Destroy,
}

public class ParticleSystemSpawner : MonoBehaviour
{
    private void Awake()
    {
        Character.OnParticleSystemToSpawn += SpawnParticleSystem;
        AffiliationTrigger.OnParticleSystemToSpawn += SpawnParticleSystem;
    }

    private void OnDestroy()
    {
        Character.OnParticleSystemToSpawn -= SpawnParticleSystem;
        AffiliationTrigger.OnParticleSystemToSpawn -= SpawnParticleSystem;
    }

    public void SpawnParticleSystem(PSProperties properties)
    {
        Transform PSTransform = Instantiate(getPSPrefab(properties.PSType), properties.PSposition, Quaternion.identity, null);
        if (PSTransform != null)
        {
            ParticleSystem particleSystem = PSTransform.GetComponent<ParticleSystem>();
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
}
