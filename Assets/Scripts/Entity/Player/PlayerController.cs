using System;
using System.Linq;

using Enemy;

using UnityEditor.Timeline;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using Utilities;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputHandler))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Stats))]
[RequireComponent(typeof(SFXEmitter))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

    [Header("Component References")]
    [SerializeField] private CharacterController _controller;
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private Health _health;
    [SerializeField] private SFXEmitter _emitter;
    [SerializeField] private Stats _stats;
    [SerializeField] private Animator _animator;
    [SerializeField] private EntityDamager _entityDamager;

    [Header("Player Variables")]
    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 7f;
    [SerializeField] private float _acceleration = 600f;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private Vector3 _velocity;
    [SerializeField] private Vector3 _input;
    [SerializeField] private CountDownTimer _lightAttack;
    [SerializeField] private CountDownTimer _heavyAttack;
    [SerializeField] private float _attackRadius = 2f;

    [Header("Gameplay Variables")]
    [SerializeField] private float _speed;
    [SerializeField] private float _eulerY;
    [SerializeField] private Vector3 _targetDirection;
    [SerializeField] private Vector3 _lookDiff;
    [SerializeField] private Quaternion _camRotation;

    [Header("UI")]
    public bool AllowInput;

    [Header("Movement Abilities")]
    [SerializeField] private float _dashCooldown = 2f;
    [SerializeField] private float _dashForce = 20f;
    [SerializeField] private CountDownTimer _dashTimer;
    [SerializeField] private bool _dashPressed = false; //Need to make sure this is consumed in FixedUpdate
    [SerializeField] private float _brakeForce = 20f;
    private readonly int _speedHash = Animator.StringToHash("Speed");
    private readonly int _lightHash = Animator.StringToHash("Light");
    private readonly int _heavyHash = Animator.StringToHash("Heavy");
    [SerializeField] private float[] _animationTimes = new float[2];

    public float SpeedPercent => Mathf.Clamp01(_speed / _maxSpeed);
    private bool IsIdle => !_lightAttack.IsRunning && !_heavyAttack.IsRunning;

    private void Awake() {
        _controller = GetComponent<CharacterController>();
        _inputHandler = GetComponent<InputHandler>();
        _entityDamager = GetComponentInChildren<EntityDamager>();
        _emitter = GetComponent<SFXEmitter>();
        _stats = GetComponent<Stats>();
        _lightAttack = new CountDownTimer(_animationTimes[0] / _stats.GetStat(StatType.ATTACK_SPEED));
        _lightAttack.OnTimerStop += EndAttack;
        _heavyAttack = new CountDownTimer(_animationTimes[1] / _stats.GetStat(StatType.ATTACK_SPEED));
        _heavyAttack.OnTimerStop += EndAttack;
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _health.Init(_stats.GetStat(StatType.MAX_HEALTH), _stats.GetStat(StatType.DEFENCE));
        _stats.OnStatChange += _health.UpdateHealthAndDefence;
        _dashTimer = new CountDownTimer(_dashCooldown);
        _dashTimer.Start();
        _entityDamager.OnHit += Attack;
    }

    private void Start() {
        _health.OnDamage += (float amount) => _emitter.Play(SoundEffectType.PLAYER_HIT, amount);
        _health.OnDeath += () => { _emitter.Play(SoundEffectType.PLAYER_DEATH); GameManager.Instance.PlayerAlive = false; };
        _health.OnDamage += (float amount) => GameManager.Instance.ResetCombatTimer();
        _health.OnDamage += (float amount) => GameManager.Instance.CameraShake(intensity: amount);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        if (GameManager.Instance.InMenu) {
            return;
        }

        if (IsIdle && AllowInput && !GameManager.Instance.InMenu) {
            if (_inputHandler.HeavyFireInput) {
                _heavyAttack.Start();
                HeavyAttack();
            } else if (_inputHandler.FireInput) {
                _lightAttack.Start();
                LightAttack();
            }
        }

        if (_inputHandler.IsDashPressed && _dashTimer.IsFinished) {
            _dashPressed = true;
        }
    }

    private void LightAttack() {
        _emitter.Play(SoundEffectType.PLAYER_LIGHT);
        _animator.SetTrigger(_lightHash);
        _entityDamager.StartAttack();
        _lightAttack.Reset(_animationTimes[0] / _stats.GetStat(StatType.ATTACK_SPEED));
        _lightAttack.Start();
        _animator.speed = _animationTimes[0] * _stats.GetStat(StatType.ATTACK_SPEED);
    }

    private void HeavyAttack() {
        _emitter.Play(SoundEffectType.PLAYER_HEAVY);
        _animator.SetTrigger(_heavyHash);
        _entityDamager.StartAttack();
        _heavyAttack.Reset(_animationTimes[1] / _stats.GetStat(StatType.ATTACK_SPEED));
        _heavyAttack.Start();
        _animator.speed = _animationTimes[1] * _stats.GetStat(StatType.ATTACK_SPEED);
    }

    public string GetStatus() {
        return 
            $"Health: {_health.CurrentHealth} / {_health.MaxHealth} ({_health.PercentHealth:0%})\n" +
            $"Defence: {_stats.GetStat(StatType.DEFENCE)}\n" +
            $"Speed: {_stats.GetStat(StatType.MOVE_SPEED)}\n" +
            $"Attack Damage: {_stats.GetStat(StatType.ATTACK_DAMAGE)}\n" +
            $"Attack Speed: {_stats.GetStat(StatType.ATTACK_SPEED)}\n"
        ;
    }

    private void EndAttack() {
        _entityDamager.EndAttack();
        _animator.speed = 1f;
    }

    private void Attack(Health health, Vector3 position) {
        if (_lightAttack.IsRunning) {
            LightAttackHit(health, position);
        } else if (_heavyAttack.IsRunning) {
            HeavyAttackHit(health, position);
        }
    }

    private void LightAttackHit(Health enemyHealth, Vector3 position) {
        Debug.Log($"Light attack on enemy {enemyHealth.name}");
        enemyHealth.Damage(_stats.GetStat(StatType.ATTACK_DAMAGE));
        Instantiate(Assets.Instance.HitParticles, position, enemyHealth.transform.rotation);
    }

    private void HeavyAttackHit(Health enemyHealth, Vector3 position) {
        Debug.Log($"Light attack on enemy {enemyHealth.name}");
        enemyHealth.Damage(_stats.GetStat(StatType.ATTACK_DAMAGE));
        Instantiate(Assets.Instance.HitParticles, position, enemyHealth.transform.rotation);
    }

    private void FixedUpdate() {
        _speed = Mathf.MoveTowards(_speed, GetTargetSpeed(), Time.fixedDeltaTime * _acceleration);
        _input.x = _inputHandler.MoveInput.x;
        _input.z = _inputHandler.MoveInput.y;
        _input.y = 0f;
        _input.Normalize();
        UpdateTargetDirection();
        Move();
        _animator.SetFloat(_speedHash, Mathf.Clamp01(_speed / GetMaxSpeed()));
        _dashTimer.Update(Time.fixedDeltaTime);
        _lightAttack.Update(Time.fixedDeltaTime);
        _heavyAttack.Update(Time.fixedDeltaTime);
    }

    private void UpdateTargetDirection() {
        _targetDirection = Quaternion.AngleAxis(Globals.Instance.MainCamera.transform.eulerAngles.y, Vector3.up) * _input;
        if (_inputHandler.IsMovePressed) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_targetDirection, transform.up), _rotationSpeed * Time.fixedDeltaTime);
        }   
    }

    private void Move() {
        _velocity = _targetDirection;
        _velocity.y = -0.1f;
        _velocity = Vector3.ClampMagnitude(_velocity * _speed, GetMaxSpeed());
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
