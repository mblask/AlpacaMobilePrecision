using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType
{
    ContractRelease,
}

public class CharacterAnimation : MonoBehaviour, ICharacterAnimation
{
    private Animator _animator;

    private string _contractReleaseTrigger = "ContractRelease";

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayAnimation(AnimationType animationType)
    {
        switch (animationType)
        {
            case AnimationType.ContractRelease:
                playContractRelease();
                break;
            default:
                break;
        }
    }

    private void playContractRelease()
    {
        _animator.SetTrigger(_contractReleaseTrigger);
    }
}
