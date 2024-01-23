using UnityEngine;

namespace Enemy {
    public class EnemyPatrolState : EnemyState {
        private int _wanderPointIndex;
        public EnemyPatrolState(EnemyController controller, EnemyManager enemyManager, EnemyStateFactory enemyStateFactory) : base(controller, enemyManager, enemyStateFactory) { }

        public override void Start() {
            _wanderPointIndex = Random.Range(0, _enemyManager.WanderPoints.Length);
            _controller.Agent.destination = _enemyManager.WanderPoints[_wanderPointIndex].position;
        }

        public override void FixedUpdate() {
            if (Vector3.Distance(_controller.Agent.destination, _controller.transform.position) <= _controller.Agent.stoppingDistance) {
                _wanderPointIndex = ++_wanderPointIndex % _enemyManager.WanderPoints.Length;
                _controller.Agent.destination = _enemyManager.WanderPoints[_wanderPointIndex].position;
            }
        }
        public override void CheckTransitions() {
            if (!_controller.HasTarget) {
                SwitchState(_enemyStateFactory.State<EnemyIdleState>());
            }
            if (_controller.InAttackRange) {
                SwitchState(_enemyStateFactory.State<EnemyAttackState>());
                return;
            } else if (_controller.InChaseRange) {
                SwitchState(_enemyStateFactory.State<EnemyChaseState>());
                return;
            }
        }
        public override void Update() { }
        public override void Exit() { 
        }

    }
}