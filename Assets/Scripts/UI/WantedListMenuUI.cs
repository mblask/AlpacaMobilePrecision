using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WantedListMenuUI : MonoBehaviour
{
    private Transform _wantedContainer;

    private GameAssets _gameAssets;

    private void Awake()
    {
        _wantedContainer = transform.Find("Container").Find("WantedListContainer").Find("Viewport").Find("Content");
    }

    private void OnEnable()
    {
        updateWantedList();
    }

    private void updateWantedList()
    {
        WantedCharacterUI[] wantedUIList = _wantedContainer.GetComponentsInChildren<WantedCharacterUI>();
        if (wantedUIList.Length != 0)
        {
            foreach (WantedCharacterUI wantedUI in wantedUIList)
            {
                Destroy(wantedUI.gameObject);
            }
        }

        List<WantedCharacter> wantedList = WantedListManager.Instance?.GetWantedCharactersList();
        List<WantedCharacter> wantedCaught = WantedListManager.Instance?.GetUnlockedWantedCharactersList();

        if (wantedList == null || wantedList.Count == 0)
            return;

        if (_gameAssets == null)
        {
            _gameAssets = GameAssets.Instance;
            if (_gameAssets == null)
                return;
        }

        for (int i = 0; i < wantedList.Count; i++)
        {
            WantedCharacterUI wantedUI = Instantiate(_gameAssets.WantedCharacterUI, _wantedContainer).GetComponent<WantedCharacterUI>();
            wantedUI.SetupUI(wantedList[i]);

            wantedUI.SetLocked(true);
            if (wantedCaught.Contains(wantedList[i]))
                wantedUI.SetLocked(false);
        }
    }
}
