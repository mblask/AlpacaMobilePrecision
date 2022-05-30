using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitManager : MonoBehaviour
{
    private GameAssets _gameAssets;

    [Header("Hit effect")]
    [SerializeField] private bool _nearbyHitEffects = false;
    [SerializeField][Range(0.0f, 2.0f)] private float _nearbyHitRadius = 1.0f;

    private void Start()
    {
        PlayerToucher.Instance.OnPlayerTouchPosition += playerToucher_OnPlayerTouch;

        _gameAssets = GameAssets.Instance;
    }

    private void OnDestroy()
    {
        PlayerToucher.Instance.OnPlayerTouchPosition -= playerToucher_OnPlayerTouch;
    }

    private void playerToucher_OnPlayerTouch(Vector3 worldPosition)
    {
        detectCharacterHit(worldPosition);
        detectHitInArea(worldPosition);
        Instantiate(_gameAssets.BulletMark, (Vector2)worldPosition, Quaternion.identity, null);
    }

    private void detectHitInArea(Vector3 worldPosition)
    {
        Collider2D[] hitInfo = Physics2D.OverlapCircleAll(worldPosition, _nearbyHitRadius);

        if (hitInfo.Length > 0)
        {
            foreach (Collider2D collider in hitInfo)
            {
                CharacterMovement characterMovement = collider.GetComponent<CharacterMovement>();

                if (characterMovement != null)
                    Debug.Log("Character affected in range: " + _nearbyHitRadius);
            }
        }
    }

    private void detectCharacterHit(Vector3 worldPosition)
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(worldPosition, Vector2.up * 0.1f);

        if (hitInfo)
        {
            IDamagable damagable = hitInfo.collider.gameObject.GetComponent<IDamagable>();

            if (damagable != null)
                damagable.DamageThis();

            AffiliationTrigger affiliationTrigger = hitInfo.collider.gameObject.GetComponent<AffiliationTrigger>();

            if (affiliationTrigger != null)
                affiliationTrigger.TriggerAffiliationSwitch();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
    }
}
