using TMPro;

using UnityEngine;
using UnityEngine.Audio;

public class Globals : Singleton<Globals> {
    public Camera MainCamera;
    public LayerMask PlayerLayer;
    public LayerMask EnemyLayer;
    public SoundManager SoundManager;
    public Controls Controls;
    public void Start() {
        SoundManager.Init();
        PlayerLayer = 1 << LayerMask.NameToLayer("Player");
        EnemyLayer = 1 << LayerMask.NameToLayer("Enemy");
        MainCamera = Camera.main;
        Controls = new Controls();
        Controls.Enable();
    }
}