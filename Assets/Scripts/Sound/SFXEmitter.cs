using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Utilities;

public class SFXEmitter : MonoBehaviour {
    [SerializeField] private Dictionary<SoundEffectType, AudioSource> _sources;
    [SerializeField] private SoundEffectType[] _effects;
    const float PITCH_BEND_AMOUNT = 10f;

    private void Start() {
        _sources = new Dictionary<SoundEffectType, AudioSource>();
        foreach (SoundEffectType type in _effects) {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            _sources.Add(type, audioSource);
            audioSource.outputAudioMixerGroup = Globals.Instance.SoundManager.SFX;
            audioSource.clip = Globals.Instance.SoundManager.GetClip(type);
        }
    }

    public void Play(SoundEffectType soundEffect) {
        if (soundEffect == SoundEffectType.NONE) {
            return;
        }
        if (_sources.ContainsKey(soundEffect)) {
            _sources[soundEffect].Play();
        }
    }

    public void Play(SoundEffectType soundEffect, float pitchRandomisation) {
        if (_sources[soundEffect].clip == null) {
            _sources[soundEffect].pitch += pitchRandomisation / PITCH_BEND_AMOUNT;
            _sources[soundEffect].Play();
            StartCoroutine(ResetClip(soundEffect));
        }
    }

    private IEnumerator ResetClip(SoundEffectType soundEffect) {
        while (_sources[soundEffect].isPlaying) {
            yield return Yielders.WaitForSeconds(0.1f);
        }
        _sources[soundEffect].pitch = 1f;
    }
}