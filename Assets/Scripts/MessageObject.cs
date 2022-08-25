using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageObject : MonoBehaviour
{
    public bool DisplayMessages = true;

    [TextArea] public List<string> Urgents;
    [TextArea] public List<string> NonUrgents;

    private void Start()
    {
        if (DisplayMessages)
        {
            foreach (string message in Urgents)
                Debug.LogError(message);
        }
    }
}
