using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageObject : MonoBehaviour
{
    public bool DisplayMessages = true;

    [TextArea]
    public List<string> Messages;

    private void Start()
    {
        if (DisplayMessages)
        {
            foreach (string message in Messages)
                Debug.LogError(message);
        }
    }
}
