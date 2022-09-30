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

    private AudioManager _audioManager;

    [SerializeField] private List<WantedCharacter> _wantedList;
    private List<WantedCharacter> _unlockedCharacters = new List<WantedCharacter>();
    private int _currentLevel;

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        _audioManager = AudioManager.Instance;

        LevelManager.Instance.OnCharacterDestroyedAtLevel += checkWantedListAtLevel;
    }

    void OnDestroy()
    {
        LevelManager.Instance.OnCharacterDestroyedAtLevel -= checkWantedListAtLevel;
    }

    public static void ResetWantedList()
    {
        _instance.resetWantedList();
    }

    private void resetWantedList()
    {
        _unlockedCharacters.Clear();
    }

    private void checkWantedListAtLevel(int level)
    {
        if (_currentLevel != level)
            _currentLevel = level;

        float chanceToKillWanted = 5.5f - (level / 15.0f);
        if (Utilities.ChanceFunc(chanceToKillWanted))
        {
            List<WantedCharacter> wantedPool = createWantedPoolFromLevel(level);

            if (wantedPool.Count == 0)
                return;

            WantedCharacter characterKilled = wantedPool.GetRandomElement();
            if (!_unlockedCharacters.Contains(characterKilled))
            {
                _unlockedCharacters.Add(characterKilled);
                OnWantedKilled?.Invoke(characterKilled.WantedName);
                _audioManager?.PlaySFXClip(SFXClipType.WantedKilled);
            }
        }
    }

    private List<WantedCharacter> createWantedPoolFromLevel(int level)
    {
        List<WantedCharacter> wantedPool = new List<WantedCharacter>();
        foreach (WantedCharacter wanted in _wantedList)
            if (level >= wanted.WantedLevel)
                wantedPool.Add(wanted);

        return wantedPool;
    }

    public void LoadWantedList(List<WantedCharacter> wantedCharacters)
    {
        if (_unlockedCharacters == null)
            _unlockedCharacters = new List<WantedCharacter>();

        foreach (WantedCharacter wantedCharacter in wantedCharacters)
            _unlockedCharacters.Add(wantedCharacter);
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
