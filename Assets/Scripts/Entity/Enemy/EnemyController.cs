using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;

using Utilities;

namespace Enemy {
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(SFXEmitter))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Health))]
    public class EnemyController : MonoBehaviour {

        [Header("Component References")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private EnemyManager _enemyManager;
        [SerializeField] private SFXEmitter _emitter;
        [SerializeField] private Animator _animator;
        [SerializeField] private EntityDamager _entityDamager;

        [Header("Enemy Parameters")]
        [SerializeField] private float _maxSpeed = 200f;
        [SerializeField] private float _maxAcceleration = 60f;
        [SerializeField] private float _maxTurnSpeed = 5f;
        [SerializeField] private float _chaseRange = 10f;
        [SerializeField] private float _attackRange = 5f;
        [SerializeField] private float _attackDuration = 3.7f;
        [SerializeField] private float _damage = 1f;

        [Header("Gameplay Variables")]
        [SerializeField] private CountDownTimer _attackTimer;
        private EnemyState _state;
        private EnemyState.EnemyStateFactory _stateFactory;

        public EnemyState State { get { return _state; } set { _state = value; } }
        public NavMeshAgent Agent { get { return _agent; } }
        public CountDownTimer AttackTimer { get { return _attackTimer; } }
        public SFXEmitter Emitter { get { return _emitter; } }
        public Animator Animator { get { return _animator; } }
        public float Damage => _damage;
        public bool InChaseRange => HasTarget && Vector3.Distance(_enemyManager.Target.OrNull()?.position ?? new Vector3(Mathf.Infinity, Mathf.Infinity), transform.position) <= _chaseRange;
        public bool InAttackRange => HasTarget && Vector3.Distance(_enemyManager.Target.OrNull()?.position ?? new Vector3(Mathf.Infinity, Mathf.Infinity), transform.position) <= _attackRange;
        public bool HasTarget => _enemyManager.Target != null;
        public float AttackRange => _attackRange;
        public EntityDamager EntityDamager => _entityDamager;
        public readonly int AttackHash = Animator.StringToHash("Attack");
        public readonly int SpeedHash = Animator.StringToHash("Speed");


        public void Awake() {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _agent.acceleration = _maxAcceleration;
            _agent.speed = _maxSpeed;
            _agent.stoppingDistance = _attackRange - 0.5f;
            _agent.angularSpeed = _maxTurnSpeed;
            _enemyManager = transform.parent.GetComponent<EnemyManager>();
            _emitter = GetComponent<SFXEmitter>();
            _attackTimer = new CountDownTimer(_attackDuration);
            _entityDamager = GetComponentInChildren<EntityDamager>();
            _attackTimer.OnTimerStop += _entityDamager.EndAttack;
            _entityDamager.OnHit += Attack;
        }

        private void Attack(Health health, Vector3 vector) {
            health.Damage(_damage);
        }

        public void Start() {
            _stateFactory = new EnemyState.EnemyStateFactory(this, _enemyManager);
            _state = _stateFactory.State<EnemyChaseState>();
            if (_state == null) {
                Debug.LogError("State was not initialised!");
            }
            _state?.Start();
            Health health = GetComponent<Health>();
            health.Init(100f, 10f);
            _attackTimer.Start();
            // health.OnDamage += (float amount) => GameManager.Instance.ResetCombatTimer();
            // health.OnDamage += (float damage) => GameManager.Instance.CameraShake(intensity: damage);
            // health.OnDamage += (float damage) => _emitter.Play(SoundEffectType.HIT);
            // health.OnDeath += () => _emitter.Play(SoundEffectType.DESTROY);
        }

        public void Update() {
            _state?.Update();
            _state?.CheckTransitions();
        }

        public void FixedUpdate() {
            _state?.FixedUpdate();
            _attackTimer.Update(Time.fixedDeltaTime);
        }
    }
}