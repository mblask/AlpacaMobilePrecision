using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float _magnitude = 0.1f;
    private float _duration = 0.15f;

    private void Start()
    {
        Character.OnCharacterDestroyed += shakeOnCharacterDestroyed;
        GameManager.Instance.OnWorldDestruction += shakeOnWorldDestruction;
    }

    private void OnDestroy()
    {
        Character.OnCharacterDestroyed -= shakeOnCharacterDestroyed;
        GameManager.Instance.OnWorldDestruction -= shakeOnWorldDestruction;
    }

    private void shakeOnWorldDestruction()
    {
        float worldMagnitude = 1.5f;
        float worldDuration = 1.0f;
        StartCoroutine(CameraShaker(worldMagnitude * _magnitude, worldDuration));
    }

    private void shakeOnCharacterDestroyed(Character character)
    {
        float posMagnitude = 1.3f;
        float negMagnitude = 1.1f;

        switch (character.GetCharacterType())
        {
            case CharacterType.Positive:
                StartCoroutine(CameraShaker(posMagnitude * _magnitude, _duration));
                break;
            case CharacterType.Negative:
                StartCoroutine(CameraShaker(negMagnitude * _magnitude, _duration));
                break;
            default:
                break;
        }
    }

    public IEnumerator CameraShaker(float magnitude, float duration)
    {
        Vector3 originalPos = transform.position;

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            float xMag = Random.Range(-1.0f, 1.0f) * magnitude;
            float yMag = Random.Range(-1.0f, 1.0f) * magnitude;

            transform.position = new Vector3(originalPos.x + xMag, originalPos.y + yMag, originalPos.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPos;
    }
}
