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
            if (Urgents.Count != 0)
            {
                foreach (string message in Urgents)
                    Debug.LogError(message);
            }

            if (NonUrgents.Count != 0)
            {
                foreach (string message in NonUrgents)
                {
                    Debug.Log(message);
                }
            }
        }
    }
}