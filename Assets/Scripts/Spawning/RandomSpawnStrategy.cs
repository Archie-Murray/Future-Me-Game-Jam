using System.Collections.Generic;
using UnityEngine;

namespace Spawning {
    public class RandomSpawnStrategy : ISpawnStrategy {
        private Transform[] _points;
        private List<Transform> _unused;

        public RandomSpawnStrategy(Transform[] points) {
            _points = points;
            _unused = new List<Transform>(points);
        }

        public Transform GetPosition() {
            Transform point = _unused[Random.Range(0, _unused.Count)];
            _unused.Remove(point);
            if (_unused.Count == 0) {
                _unused = new List<Transform>(_points);
            }
            return point;
        }
    }
}