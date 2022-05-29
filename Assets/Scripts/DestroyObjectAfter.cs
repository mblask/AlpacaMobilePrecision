using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectAfter : MonoBehaviour
{
    [SerializeField] private float _destroyAfter;

    void Start()
    {
        Destroy(this.gameObject, _destroyAfter);
    }
}
