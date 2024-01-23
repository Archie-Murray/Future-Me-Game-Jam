
using UnityEngine;

using Utilities;

namespace Enemy {
    public class EnemyAttackState : EnemyState {

        public EnemyAttackState(EnemyController controller,  EnemyManager enemyManager, EnemyStateFactory enemyStateFactory) : base(controller, enemyManager, enemyStateFactory) { }

        public override void Start() {
        }
        public override void FixedUpdate() {
            if (_controller.AttackTimer.IsFinished) {
            }
        }

        public override void CheckTransitions() {
            if (!_enemyManager.Target) {
                SwitchState(_enemyStateFactory.State<EnemyIdleState>());
                return;
            }
            if (!_controller.InAttackRange && _controller.InChaseRange) {
                SwitchState(_enemyStateFactory.State<EnemyChaseState>());
            } else if (!_controller.InChaseRange) {
                SwitchState(_enemyStateFactory.State<EnemyPatrolState>());
            }
        }

        public override void Update() { }
        public override void Exit() {
        }
    }
}