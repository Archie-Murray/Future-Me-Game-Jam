using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class Emotemanager : MonoBehaviour {
    private readonly int _emoteHash = Animator.StringToHash("Emote");
    [SerializeField] private int _emoteID = 0;
    [SerializeField] private Animator _animator;
    private bool _enabledGUI = false;
    [SerializeField] private CanvasGroup _animGUI;
    // Start is called before the first frame update
    void Start() {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        _animator.SetFloat(_emoteHash, _emoteID);
        _animGUI.alpha = _enabledGUI ? 1f : 0f;
        if (Input.GetKeyDown(KeyCode.E)) {
            _enabledGUI = !_enabledGUI;
        }
    }


    public void SetEmote(int value) {
        if (_emoteID == value) {
            _emoteID = 0;
        } else {
            _emoteID = value;
        }
    }
}
