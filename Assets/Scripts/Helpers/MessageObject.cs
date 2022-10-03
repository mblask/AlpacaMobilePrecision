using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageObject : MonoBehaviour
{
    public bool DisplayImportant = true;
    public bool DisplayLessImportant = true;

    [TextArea] public List<string> Important;
    [TextArea] public List<string> LessImportant;

    private void Start()
    {
        if (DisplayImportant)
        {
            if (Important.Count != 0)
            {
                foreach (string message in Important)
                    Debug.LogError(message);
            }
        }

        if (DisplayLessImportant)
        { 
            if (LessImportant.Count != 0)
            {
                foreach (string message in LessImportant)
                {
                    Debug.Log(message);
                }
            }
        }
    }
}
