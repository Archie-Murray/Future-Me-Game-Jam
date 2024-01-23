using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utilities;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(InputHandler))]
public class PlayerController : MonoBehaviour {

    [Header("Component References")]
    [SerializeField] private Rigidbody2D _rb2D;
    [SerializeField] private InputHandler _inputHandler;  
    [SerializeField] private Health _health;
    [SerializeField] private SFXEmitter _emitter;
    
    [Header("Player Variables")]
    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 7f;
    [SerializeField] private float _acceleration = 600f;
    [SerializeField] private float _maxRotationSpeed = 5f;
    [SerializeField] private float _rotationAcceleration = 5f;

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
        _rb2D = GetComponent<Rigidbody2D>();
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
        RotateToMouse();
        _fireTimer.Update(Time.fixedDeltaTime);
        _heavyFireTimer.Update(Time.fixedDeltaTime);
        _eliteFireTimer.Update(Time.fixedDeltaTime);
        _dashTimer.Update(Time.fixedDeltaTime);
    }

    private void RotateToMouse() {
        Vector2 lookAt = (Globals.Instance.MainCamera.ScreenToWorldPoint(_inputHandler.MousePosition) - transform.position).normalized;
        _rotationSpeed = Mathf.MoveTowards(_rotationSpeed, _maxRotationSpeed, Time.fixedDeltaTime * _rotationAcceleration * (1.5f - ((Vector2.Dot(lookAt, transform.up) + 1f) * 0.5f)));
        Quaternion rotation = Quaternion.AngleAxis(Vector2.SignedAngle(lookAt, Vector2.up), Vector3.back);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.fixedDeltaTime * _rotationSpeed);
    }

    private void Move() {
        _speed = Mathf.MoveTowards(_speed, GetTargetSpeed(), Time.fixedDeltaTime * _acceleration);
        if (!_inputHandler.BrakePressed) {
            _rb2D.velocity += Time.fixedDeltaTime * _speed * _inputHandler.MoveInput;
            _rb2D.velocity = _rb2D.velocity.ClampMagnitude(GetMaxSpeed());
        } else {
            _rb2D.velocity += (Time.fixedDeltaTime * _brakeForce * -_rb2D.velocity.normalized).ClampMagnitude(_rb2D.velocity.magnitude);
        }
        if (_dashPressed) {
            _dashPressed = false;
            _rb2D.AddForce(transform.up * _dashForce, ForceMode2D.Impulse);
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