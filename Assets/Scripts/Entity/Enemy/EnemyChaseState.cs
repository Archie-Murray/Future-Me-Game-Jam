using UnityEngine;

namespace Enemy {
    public class EnemyChaseState : EnemyState {
        public EnemyChaseState(EnemyController controller, EnemyManager enemyManager, EnemyStateFactory enemyStateFactory) : base(controller, enemyManager, enemyStateFactory) { }


        public override void Start() {
            _controller.Agent.destination = _enemyManager.Target.position;
        }
        public override void FixedUpdate() {
            _controller.Animator.SetFloat(_controller.SpeedHash, _controller.Agent.velocity.magnitude / _controller.Agent.speed);
            if (Vector2.Distance(_enemyManager.Target.position, _controller.Agent.destination) > 1f) {
                _controller.Agent.destination = _enemyManager.Target.position;
            }
        }
        public override void CheckTransitions() {
            if (!_controller.HasTarget) {
                SwitchState(_enemyStateFactory.State<EnemyIdleState>());
                return;
            }
            if (_controller.InAttackRange) {
                SwitchState(_enemyStateFactory.State<EnemyAttackState>());
            }
        }

        public override void Update() { }
        public override void Exit() {
        }

    }
}
