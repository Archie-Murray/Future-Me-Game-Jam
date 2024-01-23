using System;
using System.Runtime.InteropServices.WindowsRuntime;

using UnityEngine;
using UnityEngine.UI;

public class UIMover : MonoBehaviour {
    [SerializeField] private RectTransform _rootAnchor;
    [SerializeField] private RectTransform _root;
    [SerializeField] private CanvasGroup _rootGroup;
    [SerializeField] private KeyCode _key = KeyCode.Tab;
    [SerializeField] private Vector2 _targetPos;
    [SerializeField] private float _speed = 5;
    [SerializeField] private Vector2 _moveDir = Vector2.up;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private bool _openByDefault = false;
    [SerializeField] private bool _hasInvokedMoveFinish = false;

    public Action OnCloseStart;
    public Action OnCloseFinish;
    public Action OnOpenStart;
    public Action OnOpenFinish;

    private void Awake() {
        _root = GetComponentInChildren<UIRoot>().GetComponent<RectTransform>();
        _rootAnchor = GetComponentInChildren<UIAnchor>().GetComponent<RectTransform>();
        _rootGroup = _root.GetComponent<CanvasGroup>();

        OnCloseStart += () => ToggleUIChildrenInteractable(false);
        OnCloseFinish += () => ToggleUIChildrenVisible(false);
        OnOpenFinish += () => ToggleUIChildrenInteractable(true);
        OnOpenStart += () => ToggleUIChildrenVisible(true);

        if (_key == KeyCode.Escape) {
            OnCloseStart += () => GameManager.Instance.InMenu = false;
            OnOpenStart += () => GameManager.Instance.InMenu = true;
        }

        _targetPos = _openByDefault ? OpenPos : ClosedPos;
        ToggleUIChildrenInteractable(_openByDefault);
    }

    private Vector2 Offset => _offset.Multiply(x: Screen.width / 1920f * _moveDir.x, y: Screen.height / 1080f * _moveDir.y);
    private Vector2 OpenPos => _rootAnchor.anchoredPosition;
    private Vector2 ClosedPos => _rootAnchor.anchoredPosition + Offset;

    private void FixedUpdate() {
        if (Vector3.Distance(_root.anchoredPosition, _targetPos) <= 0.01f && !_hasInvokedMoveFinish) {
            if (_targetPos == ClosedPos) {
                Debug.Log($"UI {_key} finished closing");
                OnCloseFinish?.Invoke();
            } else {
                OnOpenFinish?.Invoke();
            }
            _hasInvokedMoveFinish = true;
            return;
        } else if (Vector3.Distance(_root.anchoredPosition, _targetPos) >= 0.01f) {
            _hasInvokedMoveFinish = false;
            _root.anchoredPosition = Vector2.MoveTowards(_root.anchoredPosition, _targetPos, _speed * Time.fixedDeltaTime);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(_key)) {
            _targetPos = _targetPos == OpenPos ? ClosedPos : OpenPos;
            if (_targetPos == ClosedPos) {
                OnCloseStart?.Invoke();
            } else {
                OnOpenStart?.Invoke();
            }
        }
    }

    private void ToggleUIChildrenInteractable(bool value) {
        _rootGroup.interactable = value;
    }

    private void ToggleUIChildrenVisible(bool value) {
        _rootGroup.alpha = value ? 1f : 0f;
    }
}