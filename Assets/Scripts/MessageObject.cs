using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageObject : MonoBehaviour
{
    [TextArea]
    public List<string> Messages;

    private void Start()
    {
        foreach (string message in Messages)
        {
            Debug.LogError(message);
        }
    }
}
