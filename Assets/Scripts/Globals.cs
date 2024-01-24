using TMPro;

using UnityEngine;
using UnityEngine.Audio;

public class Globals : Singleton<Globals> {
    public Camera MainCamera;
    public LayerMask PlayerLayer;
    public LayerMask EnemyLayer;
    public SoundManager SoundManager;
    [SerializeField] private SoundEffect[] _effects;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioMixerGroup _sfx, _bgm;

    public void Start() {
        SoundManager = new SoundManager(_effects, _audioMixer, _sfx, _bgm);
        PlayerLayer = 1 << LayerMask.NameToLayer("Player");
        EnemyLayer = 1 << LayerMask.NameToLayer("Enemy");
        MainCamera = Camera.main;
    }
}