using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGameProgressButton : MonoBehaviour
{
    private void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            gameObject.SetActive(false);
    }
}
