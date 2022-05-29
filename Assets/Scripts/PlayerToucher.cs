using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToucher : MonoBehaviour
{
    [SerializeField] private Transform _testCircle;

    private Camera _mainCamera;

    private float _touchTime;

    private void Start()
    {
        _mainCamera = Camera.main;

        _touchTime = 0.0f;
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    detectHit(Input.mousePosition);
        //}

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Vector2 worldTouchPosition = _mainCamera.ScreenToWorldPoint(touch.position);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    detectHit(worldTouchPosition);
                    Instantiate(_testCircle, worldTouchPosition, Quaternion.identity, null);
                    break;

                case TouchPhase.Moved:
                    break;

                case TouchPhase.Stationary:
                    _touchTime += Time.deltaTime;

                    if (_touchTime >= 5.0f)
                        Debug.Log("Stop touching me!");
                    break;

                case TouchPhase.Ended:
                    //Debug.Log("Touch time: " + _touchTime);
                    _touchTime = 0.0f;
                    break;

                case TouchPhase.Canceled:
                    break;

                default:
                    break;
            }
        }
    }

    private void detectHit(Vector3 worldPosition)
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
