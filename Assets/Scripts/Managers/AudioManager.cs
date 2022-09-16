using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using AlpacaMyGames;
using System;

public enum AudioType
{
    Master,
    SFX,
    Music,
}

public enum SFXClipType
{
    CharacterKilled,
    ObstacleHit,
    ObstacleSmashed,
    Victory,
    Failure,
    Spawning,
    Gunshot,
    WantedKilled,
    AchievementScored,
    Explosion,
}

public enum MusicClipType
{
    MainMenu,
}

public class AudioProperties
{
    public AudioType AudioType;
    public bool IsMuted;
    public float Volume;
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

    public event Action<AudioProperties> OnToggleMuteAudio;

    [SerializeField] private AudioMixer _mainMixer;

    [Header("Music Clips")]
    //[SerializeField] private AudioClip _mainMenuMusic;
    [SerializeField] private List<AudioClip> _musicClips;

    [Header("SFX Clips")]
    [SerializeField] private List<AudioClip> _characterKilled;
    [SerializeField] private List<AudioClip> _obstacleHit;
    [SerializeField] private List<AudioClip> _obstacleSmashed;
    [SerializeField] private List<AudioClip> _explosions;
    [SerializeField] private AudioClip _victory;
    [SerializeField] private AudioClip _failure;
    [SerializeField] private AudioClip _spawning;
    [SerializeField] private AudioClip _gunshot;
    [SerializeField] private AudioClip _wantedKilled;
    [SerializeField] private AudioClip _achievementScored;

    private string _sfxVolumeString = "SFXVolume";
    private string _musicVolumeString = "MusicVolume";

    private AudioSource _sfxSource;
    private AudioSource _musicSource;

    private bool _sfxMuted = false;
    private bool _musicMuted = false;

    private float _minDBZVolume = -80.0f;
    private float _lastSFXVolume;
    private float _lastMusicVolume;

    public float SFXVolume
    {
        get
        {
            _mainMixer.GetFloat(_sfxVolumeString, out float value);
            return Mathf.Pow(10.0f, value / 30.0f);
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
            return Mathf.Pow(10.0f, value / 30.0f);
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

    private void LateUpdate()
    {
        if (!_musicSource.isPlaying)
            PlayMusicClip();
    }

    public void PlaySFXClip(SFXClipType sfxType)
    {
        AudioClip clip;

        switch (sfxType)
        {
            case SFXClipType.CharacterKilled:
                clip = _characterKilled.GetRandomElement();
                break;
            case SFXClipType.ObstacleHit:
                clip = _obstacleHit.GetRandomElement();
                break;
            case SFXClipType.ObstacleSmashed:
                clip = _obstacleSmashed.GetRandomElement();
                break;
            case SFXClipType.Victory:
                clip = _victory;
                break;
            case SFXClipType.Failure:
                clip = _failure;
                break;
            case SFXClipType.Spawning:
                clip = _spawning;
                break;
            case SFXClipType.Gunshot:
                clip = _gunshot;
                break;
            case SFXClipType.WantedKilled:
                clip = _wantedKilled;
                break;
            case SFXClipType.AchievementScored:
                clip = _achievementScored;
                break;
            case SFXClipType.Explosion:
                clip = _explosions.GetRandomElement();
                break;
            default:
                clip = null;
                break;
        }

        if (clip != null)
            _sfxSource.PlayOneShot(clip);
    }

    public void PlayMusicClip()
    {
        _musicSource.Stop();
        _musicSource.PlayOneShot(_musicClips.GetRandomElement());
    }

    public void ToggleMuteSFX()
    {
        _sfxMuted = !_sfxMuted;

        if (_sfxMuted)
        {
            _lastSFXVolume = SFXVolume;
            SFXVolume = _minDBZVolume;
        }
        else
            SFXVolume = _lastSFXVolume;

        OnToggleMuteAudio?.Invoke(new AudioProperties { AudioType = AudioType.SFX, IsMuted = _sfxMuted, Volume = _lastSFXVolume });
    }

    public void ToggleMuteMusic()
    {
        _musicMuted = !_musicMuted;

        if (_musicMuted)
        {
            _lastMusicVolume = MusicVolume;
            MusicVolume = _minDBZVolume;
        }
        else
            MusicVolume = _lastMusicVolume;

        OnToggleMuteAudio?.Invoke(new AudioProperties { AudioType = AudioType.Music, IsMuted = _musicMuted, Volume = _lastMusicVolume });
    }
}
