using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UISmallElement : MonoBehaviour
{
    protected abstract void resetUIElementValue();

    protected abstract void activateUIElement(bool value);

    protected abstract void showUIElement();

    protected abstract void hideUIElement();
}
