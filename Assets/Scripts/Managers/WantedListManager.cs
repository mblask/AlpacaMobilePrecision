using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AlpacaMyGames;

public class WantedListManager : MonoBehaviour
{
    private static WantedListManager _instance;

    public static WantedListManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public Action<string> OnWantedKilled;

    [SerializeField] private List<WantedCharacter> _wantedList;
    private List<WantedCharacter> _unlockedCharacters = new List<WantedCharacter>();
    private int _currentLevel;

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        LevelManager.Instance.OnCharacterDestroyedAtLevel += checkWantedListAtLevel;
    }

    void OnDestroy()
    {
        LevelManager.Instance.OnCharacterDestroyedAtLevel -= checkWantedListAtLevel;
    }

    private void checkWantedListAtLevel(int level)
    {
        if (_currentLevel != level)
            _currentLevel = level;

        List<WantedCharacter> wantedPool = new List<WantedCharacter>();
        if (Utilities.ChanceFunc(10))
        {
            wantedPool = createWantedPoolFromLevel(level);

            if (wantedPool.Count == 0)
                return;

            if (Utilities.ChanceFunc(70))
            {
                WantedCharacter characterKilled = wantedPool[UnityEngine.Random.Range(0, wantedPool.Count)];
                if (!_unlockedCharacters.Contains(characterKilled))
                {
                    _unlockedCharacters.Add(characterKilled);
                    OnWantedKilled?.Invoke(characterKilled.WantedName);
                    //Debug.Log("You killed " + characterKilled.WantedName + ". Ratio: " + (float)characterKilled.WantedLevel / level);
                }
            }
        }
    }

    private List<WantedCharacter> createWantedPoolFromLevel(int level)
    {
        List<WantedCharacter> wantedPool = new List<WantedCharacter>();
        foreach (WantedCharacter wanted in _wantedList)
        {
            if (level >= wanted.WantedLevel)
                wantedPool.Add(wanted);
        }

        return wantedPool;
    }

    public List<WantedCharacter> GetWantedCharactersList()
    {
        return _wantedList;
    }

    public List<WantedCharacter> GetUnlockedWantedCharactersList()
    {
        return _unlockedCharacters;
    }
}
