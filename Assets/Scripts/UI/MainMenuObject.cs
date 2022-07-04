using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuObject : MonoBehaviour
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
