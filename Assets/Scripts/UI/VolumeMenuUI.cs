using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeMenuUI : MonoBehaviour
{
    private Slider _sfxVolumeSlider;
    private Slider _musicVolumeSlider;
    private Toggle _muteSFXToggle;
    private Toggle _muteMusicToggle;

    private AudioManager _audioManager;

    private void Awake()
    {
        _sfxVolumeSlider = transform.Find("SFXVolumeSlider").GetComponent<Slider>();
        _musicVolumeSlider = transform.Find("MusicVolumeSlider").GetComponent<Slider>();
        //_muteSFXToggle = transform.Find("MuteSFXCheckbox").GetComponent<Toggle>();
        //_muteMusicToggle = transform.Find("MuteMusicCheckbox").GetComponent<Toggle>();
    }

    private void OnEnable()
    {
        _sfxVolumeSlider.onValueChanged.AddListener(onSFXSliderValueChanged);
        _musicVolumeSlider.onValueChanged.AddListener(onMusicSliderValueChanged);
        //_muteMusicToggle.onValueChanged.AddListener(onMusicMuteToggle);
        //_muteSFXToggle.onValueChanged.AddListener(onSFXMuteToggle);
    }

    private void Start()
    {
        _audioManager = AudioManager.Instance;

        //_sfxVolumeSlider.onValueChanged.AddListener(onSFXSliderValueChanged);
        //_musicVolumeSlider.onValueChanged.AddListener(onMusicSliderValueChanged);
        //_muteMusicToggle.onValueChanged.AddListener(onMusicMuteToggle);
        //_muteSFXToggle.onValueChanged.AddListener(onSFXMuteToggle);
    }

    private void OnDisable()
    {
        _sfxVolumeSlider.onValueChanged.RemoveAllListeners();
        _musicVolumeSlider.onValueChanged.RemoveAllListeners();
        //_muteMusicToggle.onValueChanged.RemoveAllListeners();
        //_muteSFXToggle.onValueChanged.RemoveAllListeners();
    }

    private void onSFXMuteToggle(bool value)
    {
        _audioManager?.ToggleMuteSFX();
    }

    private void onSFXSliderValueChanged(float value)
    {
        if (_muteSFXToggle == null)
            return;

        if (value == _sfxVolumeSlider.minValue)
            _muteSFXToggle.isOn = true;
        else
            _muteSFXToggle.isOn = false;
    }

    private void onMusicMuteToggle(bool value)
    {
        _audioManager?.ToggleMuteMusic();
    }

    private void onMusicSliderValueChanged(float value)
    {
        if (_muteMusicToggle == null)
            return;

        if (value == _musicVolumeSlider.minValue)
            _muteMusicToggle.isOn = true;
        else
            _muteMusicToggle.isOn = false;
    }
}