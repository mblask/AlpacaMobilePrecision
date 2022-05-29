using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AlpacaUtils
{
    public static bool ChanceFunc(int percentageChance)
    {
        if (percentageChance == 0)
            return false;

        if (percentageChance == 100)
            return true;

        int randomNumber = Random.Range(0, 101);

        return randomNumber <= percentageChance;
    }

    public static Vector2 GetRandomPositionOnScreen(int shorteningFactor = 1)
    {
        Vector3 randomScreenPosition = Vector2.up * Screen.height * UnityEngine.Random.Range(0.0f, 1.0f) +
                    Vector2.right * Screen.width * UnityEngine.Random.Range(0.0f, 1.0f);
        Vector2 randomPosition = Camera.main.ScreenToWorldPoint(randomScreenPosition);

        return randomPosition / shorteningFactor;
    }

    public static Vector2 GetRandomScreenPosition(float borderScaling = 1.0f)
    {
        float x0y0 = 1.0f - borderScaling;

        return new Vector2(Random.Range(x0y0, 1.0f) * borderScaling * Screen.width, Random.Range(x0y0, 1.0f) * borderScaling * Screen.height);
    }
}
