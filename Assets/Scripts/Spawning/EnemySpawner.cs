using System;

using UnityEngine;
namespace Spawning {
    [Serializable]
    public class EnemySpawner {
        [SerializeField] private ISpawnStrategy _spawnStrategy;
        [SerializeField] GameObject _enemyPrefab;

        public EnemySpawner(ISpawnStrategy spawnStrategy, GameObject enemyPrefab) {
            _spawnStrategy = spawnStrategy;
            _enemyPrefab = enemyPrefab;
        }

        public GameObject Spawn(Transform parent = null) {
            Transform spawnPoint = _spawnStrategy.GetPosition();
            GameObject instance;
            if (parent) {
                instance = UnityEngine.Object.Instantiate(_enemyPrefab, parent);
                instance.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            } else {
                instance = UnityEngine.Object.Instantiate(_enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            }
            return instance;
        }
    }
}