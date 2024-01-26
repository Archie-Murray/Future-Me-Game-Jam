using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

[Serializable] public class SoundManager {

    public AudioMixer MainMixer;
    public AudioMixerGroup BGM;
    public AudioMixerGroup SFX;
    public SoundEffect[] SoundEffects;

    [SerializeField] private Dictionary<SoundEffectType, AudioClip> _clips = new Dictionary<SoundEffectType, AudioClip>();
    public void Init() {
        foreach (SoundEffect soundEffect in SoundEffects) {
            _clips.Add(soundEffect.Type, soundEffect.Clip);
        }
    }

    public AudioClip GetClip(SoundEffectType type) {
        if (!_clips.ContainsKey(type)) {
            Debug.LogWarning("Could not load clip " + type);
            return null;
        } else { 
            return _clips[type]; 
        }
    }
}