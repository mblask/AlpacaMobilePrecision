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

    public static Vector2 GetRandomWorldPosition(int shorteningFactor = 1)
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

    public static bool IsInsideScreen(Vector2 position)
    {
        if (position.x >= 0.0f && position.x < Screen.width && position.y >= 0.0f && position.y < Screen.height)
            return true;
        else
            return false;
    }

    public static float GetWorldCameraRatio()
    {
        return (float)Camera.main.pixelWidth / (float)Camera.main.pixelHeight;
    }

    public static Vector2 GetWorldOrthographicCameraSize()
    {
        float cameraOrthographicHeight = Camera.main.orthographicSize;

        return new Vector2(cameraOrthographicHeight * GetWorldCameraRatio(), cameraOrthographicHeight);
    }

    public static Vector2 GetDistanceToNearestWorldEdges(Vector2 position)
    {
        Vector2 worldSize = GetWorldOrthographicCameraSize();
        if (position.x > 0.0f)
        {
            if (position.y > 0.0f)
            {
                float vectorX = worldSize.x - position.x;
                float vectorY = worldSize.y - position.y;
                return new Vector2(vectorX, vectorY);
            }
            else if (position.y < 0.0f)
            {

                float vectorX = worldSize.x - position.x;
                float vectorY = worldSize.y - Mathf.Abs(position.y);
                return new Vector2(vectorX, vectorY);
            }
            else
            {
                float vectorX = worldSize.x - position.x;
                float vectorY = worldSize.y;
                return new Vector2(vectorX, vectorY);
            }
        }
        else if (position.x < 0.0f)
        {
            if (position.y > 0.0f)
            {
                float vectorX = worldSize.x - Mathf.Abs(position.x);
                float vectorY = worldSize.y - position.y;
                return new Vector2(vectorX, vectorY);
            }
            else if (position.y < 0.0f)
            {
                float vectorX = worldSize.x - Mathf.Abs(position.x);
                float vectorY = worldSize.y - Mathf.Abs(position.y);
                return new Vector2(vectorX, vectorY);
            }
            else
            {
                float vectorX = worldSize.x - Mathf.Abs(position.x);
                float vectorY = worldSize.y;
                return new Vector2(vectorX, vectorY);
            }
        }
        else
        {
            if (position.y > 0.0f)
            {
                float vectorX = worldSize.x;
                float vectorY = worldSize.y - position.y;
                return new Vector2(vectorX, vectorY);
            }
            else if (position.y < 0.0f)
            {
                float vectorX = worldSize.x;
                float vectorY = worldSize.y - Mathf.Abs(position.y);
                return new Vector2(vectorX, vectorY);
            }
            else
            {
                float vectorX = worldSize.x;
                float vectorY = worldSize.y;
                return new Vector2(vectorX, vectorY);
            }
        }
    }
}

public static class MyExtensions
{
    public static Vector3 InvertVector(this Vector3 vector)
    {
        return new Vector3(-vector.x, -vector.y, -vector.z);
    }

    public static Vector2 InvertVector(this Vector2 vector)
    {
        return new Vector2(-vector.x, -vector.y);
    }
}
