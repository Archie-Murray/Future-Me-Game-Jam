using UnityEngine;

namespace Spawning {
    public class LinearSpawnStrategy : ISpawnStrategy {
        private Transform[] _offsets;
        private int _currentIndex = 0;

        public LinearSpawnStrategy(Transform[] offsets) {
            _offsets = offsets;
        }
        public Transform GetPosition() {
            int prevIndex = _currentIndex;
            _currentIndex = ++_currentIndex % _offsets.Length;
            return _offsets[prevIndex];
        }

    }
}