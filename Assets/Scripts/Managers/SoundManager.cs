using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public sealed class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField]
    private AudioMixer _audioMixer;

    private readonly List<AudioSource> _audioSources = new();

    protected override void Init()
    {
        base.Init();
        InitAudioMixer();
        CreateAudioSources();
    }

    public static void Play2D(string key, SoundType soundType)
    {
        ResourceManager.LoadAsync<AudioClip>(key, clip => Play2D(clip, soundType));
    }

    public static void Play2D(AudioClip clip, SoundType soundType)
    {
        if (clip == null)
        {
            return;
        }

        var audioSource = Instance._audioSources[(int)soundType];

        if (soundType == SoundType.BGM)
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

    public static void Stop2D(SoundType soundType)
    {
        var audioSource = Instance._audioSources[(int)soundType];
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public static void Play3D(AudioClip clip, SoundType type, Vector3 position, Transform parent = null, float minDistance = 0f, float maxDistance = 15f)
    {
        if (clip == null)
        {
            return;
        }

        ResourceManager.InstantiateAsync<DDDSoundPlayer>("DDDSoundPlayer", soundPlayer =>
        {
            var audioSource = Instance._audioSources[(int)type];
            soundPlayer.transform.localPosition = position;
            soundPlayer.Play(clip, audioSource.outputAudioMixerGroup, minDistance, maxDistance);
        }
        , parent, true);
    }

    public static float GetVolume(SoundType type)
    {
        return Instance.GetVolume(type.ToString());
    }

    public static void SetVolume(SoundType type, float volume)
    {
        Instance.SetVolume(type.ToString(), volume);
    }

    public static void Clear()
    {
        foreach (var audioSource in Instance._audioSources)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }

        PoolManager.RemovePool("DDDSoundPlayer");
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

    private void InitAudioMixer()
    {
        if (_audioMixer == null)
        {
            _audioMixer = Resources.Load<AudioMixer>("AudioMixer");
            if (_audioMixer == null)
            {
                Debug.LogWarning($"[SoundManager] AudioMixer does not exist. " +
                    $"Set a reference or create an audio mixer named AudioMixer in the resources folder.");
            }
        }
    }

    private void CreateAudioSources()
    {
        foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
        {
            if (type == SoundType.Master)
            {
                continue;
            }

            var typeName = type.ToString();
            var go = new GameObject(typeName);
            var audioSource = go.AddComponent<AudioSource>();
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
            go.transform.SetParent(transform);
        }

        _audioSources[(int)SoundType.BGM].loop = true;
    }
}
