using System;

using UnityEngine;

using Utilities;
using Spawning;

namespace Enemy {

    public class EnemyManager : MonoBehaviour {
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private Transform _target;
        [SerializeField] private int _spawnCount = 0;
        [SerializeField] private int _maxSpawnCount = 10;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private CountDownTimer _spawnTimer = new CountDownTimer(5f);
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private AudioSource _deathsound;
        [SerializeField] private GameObject _healthPickup;

        public Transform Target { get { return _target; } }
        public bool FinishedSpawning => _spawnCount == _maxSpawnCount;
        public Action OnSpawnFinish;

        private void Awake() {
            _target = FindFirstObjectByType<PlayerController>().transform;
            _spawnPoints = Array.ConvertAll(GetComponentsInChildren<EnemySpawnPoint>(), (EnemySpawnPoint point) => point.transform);
            _enemySpawner = new EnemySpawner(new RandomSpawnStrategy(_spawnPoints), _enemyPrefab);
            _deathsound = GetComponent<AudioSource>();
        }

        private void FixedUpdate() {
            _spawnTimer.Update(Time.fixedDeltaTime);
            if (_spawnTimer.IsFinished && _spawnCount < _maxSpawnCount) {
                Spawn();
                _spawnCount++;
                _spawnTimer.Reset();
                _spawnTimer.Start();
                if (_spawnCount == _maxSpawnCount) {
                    OnSpawnFinish?.Invoke();
                }
            }
        }

        public void Spawn() {
            _enemySpawner.Spawn(transform).GetComponent<Health>().OnDeath += EnemyKillReward;
        }

        private void EnemyKillReward() {
            _deathsound.Play();
            Instantiate(_healthPickup, transform);
        }
    }
}