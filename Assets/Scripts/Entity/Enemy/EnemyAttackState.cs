
using System.Linq;

using UnityEngine;

using Utilities;

namespace Enemy {
    public class EnemyAttackState : EnemyState {
        private readonly int _attackHash = Animator.StringToHash("Attack");

        public EnemyAttackState(EnemyController controller,  EnemyManager enemyManager, EnemyStateFactory enemyStateFactory) : base(controller, enemyManager, enemyStateFactory) { }

        public override void Start() {
        }
        public override void FixedUpdate() {
            if (_controller.AttackTimer.IsFinished) {
                Attack();
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
                SwitchState(_enemyStateFactory.State<EnemyIdleState>());
            }
        }

        public override void Update() { }
        public override void Exit() { }

        public void Attack() {
            Health player = Physics.OverlapSphere(_controller.transform.position, _controller.AttackRange, Globals.Instance.PlayerLayer).FirstOrDefault((Collider collider) => collider.gameObject.HasComponent<PlayerController>()).OrNull()?.GetComponent<Health>() ?? null;
            if (player) {
                _controller.Animator.SetTrigger(_attackHash);
                player.Damage(_controller.Damage);
                // Create hit particles
            }
        }
    }
}