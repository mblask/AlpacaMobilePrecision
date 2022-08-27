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
    public Transform ObstacleDestroyer;
    public Transform DestructionArea;

    [Header("UI Elements")]
    public RectTransform WantedCharacterUI;
    public RectTransform AchievementUI;

    [Header("Particle Systems")]
    public Transform BulletMarkPS;
    public Transform DestroyObjectPS;
    public Transform GlobalDestructionPS;

    private void Awake()
    {
        _instance = this;
    }
}
