using UnityEngine;

namespace Spawning {
    public interface ISpawnStrategy {
        Transform GetPosition();
    }
}