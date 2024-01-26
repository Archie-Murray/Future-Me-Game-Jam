using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static UnityEngine.Rendering.DebugUI;

public class EmoteManager : MonoBehaviour {
    private readonly int _emoteHash = Animator.StringToHash("Emote");
    private readonly int _emoteEnabledHash = Animator.StringToHash("Emote Enabled");
    private readonly int _emoteDisabledHash = Animator.StringToHash("Emote Disabled");
    [SerializeField, Range(0, 4)] private int _emoteID = 0;
    private Animator _animator;
    private bool _enabledGUI = false;
    private CanvasGroup _animGUI;
    private PlayerController _playerController;
    // Start is called before the first frame update
    void Start() {
        _playerController = FindFirstObjectByType<PlayerController>();
        _animator = _playerController.GetComponent<Animator>();
        _animGUI = GetComponent<CanvasGroup>();
        _enabledGUI = _animGUI.alpha == 1f;
        _animGUI.blocksRaycasts = _enabledGUI;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            _enabledGUI = !_enabledGUI;
            _animGUI.FadeCanvas(0.15f, !_enabledGUI, this);
            if (_enabledGUI) {
                _playerController.AllowInput = false;
                _animator.SetTrigger(_emoteEnabledHash);
                _animator.speed = 1f;
            } else {
                _playerController.AllowInput = true;
                _animator.SetTrigger(_emoteDisabledHash);
                _animator.SetFloat(_emoteHash, 0f);
            }
        }
    }


    public void SetEmote(int value) {
        if (_emoteID == value) {
            _emoteID = 0;
        } else {
            _emoteID = value;
        }
        Debug.Log(value);
        _animator.SetFloat(_emoteHash, _emoteID);
    }
}
