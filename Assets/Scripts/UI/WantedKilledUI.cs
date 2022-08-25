using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WantedKilledUI : MonoBehaviour
{
    private Animator _animator;
    private TextMeshProUGUI _title;

    private string _idleUp = "IdleUp";
    private string _pullDown = "PullDown";
    private string _pullUp = "PullUp";

    private float _pullUpTime = 1.5f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _title = transform.Find("WantedName").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        WantedListManager.Instance.OnWantedKilled += setupUI;
    }

    private void OnDestroy()
    {
        WantedListManager.Instance.OnWantedKilled -= setupUI;
    }

    private void setupUI(string title)
    {
        _title.SetText(title);

        activateUI();
    }

    private void activateUI()
    {
        _animator.SetTrigger(_pullDown);

        Invoke(nameof(deactivateUI), _pullUpTime);
    }

    private void deactivateUI()
    {
        _animator.SetTrigger(_pullUp);
    }

    private void forceDeactivateUI()
    {
        _animator.ResetTrigger(_pullDown);
        _animator.ResetTrigger(_pullUp);

        _animator.Play(_idleUp);
    }
}
