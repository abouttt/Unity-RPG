using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{
    BGM,
    SFX,
}

public class SoundManager : MonoSingleton<SoundManager>, IManager
{
    public float MasterVolume
    {
        get => GetVolume("Master");
        set => SetVolume("Master", value);
    }

    private AudioMixer _audioMixer;
    private readonly List<AudioSource> _audioSources = new();

    public void Initialize()
    {
        LoadAudioMixer();
        CreateAudioSources();
    }

    public void Play2D(string key, SoundType type)
    {
        ResourceManager.Instance.LoadAsync<AudioClip>(key, clip => Play2D(clip, type));
    }

    public void Play2D(AudioClip clip, SoundType type)
    {
        if (clip == null)
        {
            return;
        }

        var audioSource = _audioSources[(int)type];

        if (type == SoundType.BGM)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void Stop2D(SoundType type)
    {
        var audioSource = _audioSources[(int)type];
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void Play3D(AudioClip clip, SoundType type, Vector3 position, Transform parent = null,
        float minDistance = 0f, float maxDistance = 15f)
    {
        if (clip == null)
        {
            return;
        }

        ResourceManager.Instance.InstantiateAsync<DDDSoundPlayer>("DDDSoundPlayer", soundPlayer =>
        {
            var audioSource = _audioSources[(int)type];
            soundPlayer.transform.localPosition = position;
            soundPlayer.Play(clip, audioSource.outputAudioMixerGroup, minDistance, maxDistance);
        }
        , parent, true);
    }

    public float GetVolume(SoundType type)
    {
        return GetVolume(type.ToString());
    }

    public void SetVolume(SoundType type, float volume)
    {
        SetVolume(type.ToString(), volume);
    }

    public void Clear()
    {
        foreach (var audioSource in _audioSources)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }

        PoolManager.Instance.RemovePool("DDDSoundPlayer");
    }

    private float GetVolume(string name)
    {
        _audioMixer.GetFloat(name, out float dB);
        return DecibelToLinear(dB);
    }

    private void SetVolume(string name, float volume)
    {
        float linear = Mathf.Clamp(volume, 0f, 1f);
        _audioMixer.SetFloat(name, LinearToDecibel(linear));
    }

    private float LinearToDecibel(float linear)
    {
        return linear != 0f ? Mathf.Log10(linear) * 20f : -144f;
    }

    private float DecibelToLinear(float dB)
    {
        return Mathf.Pow(10f, dB / 20f);
    }

    private void LoadAudioMixer()
    {
        _audioMixer = Resources.Load<AudioMixer>("Core/AudioMixer");
        if (_audioMixer == null)
        {
            Debug.LogWarning($"[SoundManager] AudioMixer does not exist. " +
                $"Make sure there is an audio mixer named \"AudioMixer\" in your Resources/Core folder.");
        }
    }

    private void CreateAudioSources()
    {
        foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
        {
            var typeName = type.ToString();
            var newGameObject = new GameObject(typeName);
            var audioSource = newGameObject.AddComponent<AudioSource>();
            var group = _audioMixer.FindMatchingGroups(typeName);

            if (group.Length > 0)
            {
                audioSource.outputAudioMixerGroup = group[0];
            }
            else
            {
                Debug.LogWarning($"[SoundManager] {typeName} audio mixer group does not exist.");
            }

            _audioSources.Add(audioSource);
            newGameObject.transform.SetParent(transform);
        }

        _audioSources[(int)SoundType.BGM].loop = true;
    }
}
