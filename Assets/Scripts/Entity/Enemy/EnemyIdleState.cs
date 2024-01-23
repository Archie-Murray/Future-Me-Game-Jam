using UnityEngine;

namespace Enemy {
    public class EnemyIdleState : EnemyState {
        public EnemyIdleState(EnemyController controller, EnemyManager enemyManager, EnemyStateFactory enemyStateFactory) : base(controller, enemyManager, enemyStateFactory) { }

        public override void Start() { 
        }

        public override void FixedUpdate() { }
        public override void Update() { }
        public override void CheckTransitions() { }
        public override void Exit() { }
    }
}