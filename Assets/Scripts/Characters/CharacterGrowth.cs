using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGrowth : MonoBehaviour
{
    private float _originalSize;

    [SerializeField] private bool _isGrowing = false;

    private void Awake()
    {
        _originalSize = GetCharacterScale();
    }

    private void LateUpdate()
    {
        if (_isGrowing)
            growCharacterEqually();
    }

    private void growCharacterEqually()
    {
        float localScale = GetCharacterScale();

        localScale += Time.deltaTime * 0.1f;
        SetCharacterScale(localScale);

        if (localScale > _originalSize)
            _isGrowing = false;
    }

    public float GetCharacterScale()
    {
        return transform.localScale.x;
    }

    public float GetOriginalScale()
    {
        return _originalSize;
    }

    public void SetCharacterScale(float localScale)
    {
        if (localScale < _originalSize)
            _isGrowing = true;

        Vector3 scale = new Vector3(localScale, localScale, transform.localScale.z);
        transform.localScale = scale;
    }
}
