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

    private void Awake()
    {
        _sfxVolumeSlider = transform.Find("SFXVolumeSlider").GetComponent<Slider>();
        _musicVolumeSlider = transform.Find("MusicVolumeSlider").GetComponent<Slider>();
        _muteSFXToggle = transform.Find("MuteSFXCheckbox").GetComponent<Toggle>();
        _muteMusicToggle = transform.Find("MuteMusicCheckbox").GetComponent<Toggle>();
    }

    private void Start()
    {
        _sfxVolumeSlider.onValueChanged.AddListener(onSFXSliderValueChanged);
        _musicVolumeSlider.onValueChanged.AddListener(onMusicSliderValueChanged);
    }

    private void OnDisable()
    {
        _sfxVolumeSlider.onValueChanged.RemoveAllListeners();
        _musicVolumeSlider.onValueChanged.RemoveAllListeners();
    }

    private void onSFXSliderValueChanged(float value)
    {
        if (value == _sfxVolumeSlider.minValue)
            _muteSFXToggle.isOn = true;
        else
            _muteSFXToggle.isOn = false;
    }

    private void onMusicSliderValueChanged(float value)
    {
        if (value == _musicVolumeSlider.minValue)
            _muteMusicToggle.isOn = true;
        else
            _muteMusicToggle.isOn = false;
    }
}