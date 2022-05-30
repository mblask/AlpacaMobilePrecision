using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance;

    public static GameAssets Instance
    {
        get
        {
            return _instance;
        }
    }

    public Transform BulletMark;
    public Transform AffiliationTrigger;
    public Transform CharacterObject;
    public Transform ObstacleObject;

    [Header("Particle Systems")]
    public Transform DestroyObjectPS;

    private void Awake()
    {
        _instance = this;
    }
}
