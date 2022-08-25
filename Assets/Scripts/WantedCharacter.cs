using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wanted Character", menuName = "Scriptable Objects / Wanted")]
public class WantedCharacter : ScriptableObject
{
    public string WantedName;
    public int WantedLevel;
    public Sprite WantedImage;
}
