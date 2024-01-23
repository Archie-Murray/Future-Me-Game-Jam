using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utilities;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputHandler))]
public class PlayerController : MonoBehaviour {

    [Header("Component References")]
    [SerializeField] private CharacterController _controller;
    [SerializeField] private InputHandler _inputHandler;  
    [SerializeField] private Health _health;
    [SerializeField] private SFXEmitter _emitter;
    
    [Header("Player Variables")]
    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 7f;
    [SerializeField] private float _acceleration = 600f;
    [SerializeField] private float _maxRotationSpeed = 5f;
    [SerializeField] private float _rotationAcceleration = 5f;
    [SerializeField] private Vector3 _velocity;
    [SerializeField] private Vector3 _input;

    [Header("Gameplay Variables")]
    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;

    [Header("Projectiles")]
    [SerializeField] private float _fireRate = 0.25f;
    [SerializeField] private float _heavyFireRate = 0.5f;
    [SerializeField] private float _specialFireRate = 1.0f;
    [SerializeField] private CountDownTimer _fireTimer;
    [SerializeField] private CountDownTimer _heavyFireTimer;
    [SerializeField] private CountDownTimer _eliteFireTimer;

    [Header("UI")]

    [Header("Movement Abilities")]
    [SerializeField] private float _dashCooldown = 2f;
    [SerializeField] private float _dashForce = 20f;
    [SerializeField] private CountDownTimer _dashTimer;
    [SerializeField] private bool _dashPressed = false; //Need to make sure this is consumed in FixedUpdate
    [SerializeField] private float _brakeForce = 20f;

    public float SpeedPercent => Mathf.Clamp01(_speed / _maxSpeed);

    private void Awake() {
        _controller = GetComponent<CharacterController>();
        _inputHandler = GetComponent<InputHandler>();
        _emitter = GetComponent<SFXEmitter>();
        _health = GetComponent<Health>();
        _fireTimer = new CountDownTimer(_fireRate);
        _heavyFireTimer = new CountDownTimer(_heavyFireRate);
        _eliteFireTimer = new CountDownTimer(_specialFireRate);
        _dashTimer = new CountDownTimer(_dashCooldown);
        _dashTimer.Start();
    }

    private void Start() {
        _health.OnDamage += (float amount) => _emitter.Play(SoundEffectType.HIT, amount);
        _health.OnDeath += () => { _emitter.Play(SoundEffectType.DESTROY); GameManager.Instance.PlayerAlive = false; };
        _health.OnDamage += (float amount) => GameManager.Instance.ResetCombatTimer();
        _health.OnDamage += (float amount) => GameManager.Instance.CameraShake(intensity: amount);
    }

    private void Update() {
        if (GameManager.Instance.InMenu) {
            return;
        }
        
        if (_inputHandler.IsDashPressed && _dashTimer.IsFinished) {
            _dashPressed = true;
        }
    }

    private void FixedUpdate() {
        Move();
        _fireTimer.Update(Time.fixedDeltaTime);
        _heavyFireTimer.Update(Time.fixedDeltaTime);
        _eliteFireTimer.Update(Time.fixedDeltaTime);
        _dashTimer.Update(Time.fixedDeltaTime);
    }

    private void Move() {
        _speed = GetTargetSpeed();
        _input.x = _inputHandler.MoveInput.x;
        _input.z = _inputHandler.MoveInput.y;
        _velocity.y = -0.1f;
        if (!_dashTimer.IsRunning) {
            _velocity = Vector3.ClampMagnitude(_input * _speed, _maxSpeed);
        } else {
            _velocity /=_brakeForce;
        }
        _controller.SimpleMove(_velocity);
        if (_dashPressed) {
            _dashPressed = false;
            _velocity += transform.forward * _dashForce;
            GameManager.Instance.CameraPan(_dashForce * 0.1f, _dashCooldown);
            GameManager.Instance.CameraAberration(0.25f, _dashCooldown);
            _dashTimer.Reset();
            _dashTimer.Start();
        }
    }

    private float GetTargetSpeed() {
        if (!_inputHandler.IsMovePressed) {
            return 0f;
        } else {
            return _inputHandler.IsSprintPressed ? _sprintSpeed : _maxSpeed;
        }
    }

    private float GetMaxSpeed() {
        if (_dashTimer.IsRunning) {
            return _dashForce;
        } else {
            return _inputHandler.IsSprintPressed ? _sprintSpeed : _maxSpeed;
        }
    }
}