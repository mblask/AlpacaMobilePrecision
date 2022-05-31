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

    private void playerToucher_OnPlayerTouch(Vector2 worldPosition)
    {
        detectCharacterHit(worldPosition);
        Instantiate(_gameAssets.BulletMark, (Vector2)worldPosition, Quaternion.identity, null);
    }

    private void detectCharacterHit(Vector2 worldPosition)
    {
        //RaycastHit2D hitInfo = Physics2D.Raycast(worldPosition, Vector2.up, 0.1f);

        RaycastHit2D[] hitInfoArray = Physics2D.RaycastAll(worldPosition, Vector2.up, 0.1f);
        if (hitInfoArray.Length >= 1)
        {
            bool obstacleExists = false;

            foreach (RaycastHit2D hit in hitInfoArray)
            {
                Obstacle obstacle = hit.collider.GetComponent<Obstacle>();
                if (obstacle != null)
                {
                    obstacleExists = true;
                    obstacle.DamageThis();
                    break;
                }
            }

            if (!obstacleExists)
            {
                Collider2D firstCollider = hitInfoArray[0].collider;

                IDamagable damagable = firstCollider.GetComponent<IDamagable>();

                if (damagable != null)
                {
                    IAffiliationTrigger affiliationTrigger = firstCollider.GetComponent<IAffiliationTrigger>();
                    if (affiliationTrigger != null)
                        affiliationTrigger.TriggerAffiliationSwitch();

                    damagable.DamageThis();
                }
            }
        }

        /*
        if (hitInfo)
        {
            IDamagable damagable = hitInfo.collider.GetComponent<IDamagable>();

            if (damagable != null)
                damagable.DamageThis();

            AffiliationTrigger affiliationTrigger = hitInfo.collider.GetComponent<AffiliationTrigger>();

            if (affiliationTrigger != null)
                affiliationTrigger.TriggerAffiliationSwitch();
        }
        */
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
    }
}
