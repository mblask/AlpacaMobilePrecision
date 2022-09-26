using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointReachedUI : MonoBehaviour
{
    private Animator _animator;

    private string _toDefault = "ToDefault";
    private string _pullDown = "PullDown";
    private string _pullUp = "PullUp";

    private float _idleTime = 1.5f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        LevelManager.Instance.OnCheckpointReached += activateUI;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnCheckpointReached -= activateUI;
    }

    private void activateUI()
    {
        _animator.SetTrigger(_toDefault);
        _animator.SetTrigger(_pullUp);

        Invoke(nameof(deactivateUI), _idleTime);
    }

    private void deactivateUI()
    {
        _animator.SetTrigger(_pullDown);
    }
}
