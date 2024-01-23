using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Audio;

using Utilities;

public class BGMEmitter : MonoBehaviour {
    [SerializeField] private BGM[] _bgmList;

    [SerializeField] private BGMType _currentlyPlaying = BGMType.NONE;
    [SerializeField] private BGMType _target = BGMType.NONE;
    [SerializeField] private Dictionary<BGMType, AudioSource> _audioSources = new Dictionary<BGMType, AudioSource>();
    [SerializeField] private BGMType _initialBGM;
    [SerializeField] private AudioMixerGroup _mixerBGM;
    private Coroutine _mix;

    private void Awake() {
        foreach (BGM bgm in _bgmList) { 
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.clip = bgm.Clip;
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = _mixerBGM;
            _audioSources.Add(bgm.Type, audioSource);
        }
    }

    private void Start() {
        if (_initialBGM != BGMType.NONE) {
            PlayBGM(_initialBGM);
        }
    }

    public void PlayBGM(BGMType type, float fadeDuration = 5f) {
        if ((type == _target || type == _currentlyPlaying) || type == BGMType.NONE) {
            return;
        }
        _target = type;
        if (_mix != null) {
            StopCoroutine(_mix);
        } 
        _mix = StartCoroutine(MixBgm(fadeDuration));
    }

    private IEnumerator MixBgm(float duration) {
        if (_currentlyPlaying == _target) {
            yield break;
        }
        _audioSources.TryGetValue(_currentlyPlaying, out AudioSource current);
        AudioSource target = _audioSources[_target];
        //Debug.Log($"Current: {current.OrNull()?.clip.name ?? "null"}, Target: {target.OrNull()?.clip.name ?? "null"}");
        CountDownTimer timer = new CountDownTimer(duration);
        target.volume = 0f;
        target.Play();
        timer.Start();
        if (current == null) {
            Debug.Log("Only changing target");
            while (timer.IsRunning) {
                timer.Update(Time.fixedDeltaTime);
                target.volume = 1f - timer.Progress;
                yield return Yielders.WaitForFixedUpdate;
            }
        } else {
            Debug.Log("Changing both");
            while (timer.IsRunning) {
                timer.Update(Time.fixedDeltaTime);
                current.volume = timer.Progress;
                target.volume = 1f - timer.Progress;
                yield return Yielders.WaitForFixedUpdate;
            }
            current.Stop();
            current.volume = 1f;
        }
        _currentlyPlaying = _target;
    }
}

[Serializable] public enum BGMType { NONE, MAIN_MENU, PASSIVE, COMBAT }
[Serializable] public class BGM {
    public BGMType Type;
    public AudioClip Clip;
}