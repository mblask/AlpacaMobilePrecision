using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterAnimations
{
    Spawn,
}

public class CharacterAnimation : MonoBehaviour
{
    private Animator _animator;

    private bool _animationRunning = false;

    private void Awake()
    {
        _animator = GetComponentInParent<Animator>();
    }

    private void Start()
    {
        //InvokeRepeating(nameof(TestAnimation), 2.0f, 1.0f);
    }

    public void TestAnimation()
    {
        string triggerString = "SpawnCharacter";
        _animator.ResetTrigger(triggerString);
        _animator.SetTrigger(triggerString);
    }

    private void LateUpdate()
    {
        if (_animationRunning)
            spawnAnimation();
    }

    public void ActivateAnimation(float duration = 1.0f)
    {
        if (_animationRunning)
            return;

        _animationRunning = true;

        Invoke(nameof(deactivateAnimation), duration);
    }

    private void deactivateAnimation()
    {
        _animationRunning = false;
    }

    private void spawnAnimation()
    {
        float localScale = getLocalScale();
        float amplitude = 0.1f;

        float pi = Mathf.PI;
        float onePeriodPerSecond = 2 * pi;

        float change = amplitude * Mathf.Sin(onePeriodPerSecond * Time.time);

        localScale += change;

        Vector3 localScaleVector = new Vector3(localScale, localScale, transform.localScale.z);
        transform.localScale = localScaleVector;
    }

    private float getLocalScale()
    {
        return transform.localScale.x;
    }

    private void setLocalScale(float scale)
    {
        Vector3 localScale = new Vector3(scale, scale, transform.localScale.z);
        transform.localScale = localScale;
    }
}
