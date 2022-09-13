using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using AlpacaMyGames;

public enum AudioType
{
    CharacterKilled,
    ObstacleHit,
    ObstacleSmashed,
    Victory,
    Failure,
    Spawning,
    Disolving,
    Gunshot,
    WantedKilled,
}

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get
        {
            return _instance;
        }
    }
    [SerializeField] private AudioMixer _mainMixer;

    [Header("Audio Clips")]
    [SerializeField] private List<AudioClip> _characterKilled;
    [SerializeField] private List<AudioClip> _obstacleHit;
    [SerializeField] private List<AudioClip> _obstacleSmashed;
    [SerializeField] private AudioClip _victory;
    [SerializeField] private AudioClip _failure;
    [SerializeField] private AudioClip _spawning;
    [SerializeField] private AudioClip _dissolving;
    [SerializeField] private AudioClip _gunshot;
    [SerializeField] private AudioClip _wantedKilled;

    private string _sfxVolumeString = "SFXVolume";
    private string _musicVolumeString = "MusicVolume";

    private AudioSource _sfxSource;
    private AudioSource _musicSource;

    private bool _sfxMuted = false;
    private bool _musicMuted = false;
    private float _previousSFXVolume;
    private float _previousMusicVolume;

    public float SFXVolume
    {
        get
        {
            _mainMixer.GetFloat(_sfxVolumeString, out float value);
            return value;
        }

        set
        {
            _mainMixer.SetFloat(_sfxVolumeString, Mathf.Log10(value) * 30.0f);
        }
    }

    public float MusicVolume
    {
        get
        {
            _mainMixer.GetFloat(_musicVolumeString, out float value);
            return value;
        }

        set
        {
            _mainMixer.SetFloat(_musicVolumeString, Mathf.Log10(value) * 30.0f );
        }
    }

    private void Awake()
    {
        _instance = this;

        _sfxSource = transform.Find("SFXSource").GetComponent<AudioSource>();
        _musicSource = transform.Find("MusicSource").GetComponent<AudioSource>();
    }

    public void PlaySFXClip(AudioType audioType)
    {
        AudioClip clip;

        switch (audioType)
        {
            case AudioType.CharacterKilled:
                clip = _characterKilled.GetRandomElement();
                break;
            case AudioType.ObstacleHit:
                clip = _obstacleHit.GetRandomElement();
                break;
            case AudioType.ObstacleSmashed:
                clip = _obstacleSmashed.GetRandomElement();
                break;
            case AudioType.Victory:
                clip = _victory;
                break;
            case AudioType.Failure:
                clip = _failure;
                break;
            case AudioType.Spawning:
                clip = _spawning;
                break;
            case AudioType.Disolving:
                clip = _dissolving;
                break;
            case AudioType.Gunshot:
                clip = _gunshot;
                break;
            case AudioType.WantedKilled:
                clip = _wantedKilled;
                break;
            default:
                clip = null;
                break;
        }

        if (clip != null)
            _sfxSource.PlayOneShot(clip);
    }

    public void ToggleMuteSFX()
    {
        _sfxMuted = !_sfxMuted;
        SFXVolume = _sfxMuted ? -80.0f : 0.0f;
    }

    public void ToggleMuteMusic()
    {
        _musicMuted = !_musicMuted;
        MusicVolume = _musicMuted ? -80.0f : 0.0f;
    }
}
