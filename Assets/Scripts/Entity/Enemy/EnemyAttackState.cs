
using System.Linq;

using UnityEditor.Build;

using UnityEngine;

using Utilities;

namespace Enemy {
    public class EnemyAttackState : EnemyState {

        public EnemyAttackState(EnemyController controller,  EnemyManager enemyManager, EnemyStateFactory enemyStateFactory) : base(controller, enemyManager, enemyStateFactory) { }

        public override void Start() {
            Debug.Log($"Enemy {_controller.name} entered attack state");
            Attack();
            _controller.Agent.updateRotation = false;
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
            if (!_controller.InAttackRange && !_controller.AttackTimer.IsFinished && _controller.InChaseRange) {
                SwitchState(_enemyStateFactory.State<EnemyChaseState>());
            } else if (!_controller.InChaseRange) {
                SwitchState(_enemyStateFactory.State<EnemyIdleState>());
            }
        }

        public override void Update() { }
        public override void Exit() {
            _controller.Agent.updateRotation = true;
        }

        public void Attack() {
            _controller.Animator.SetTrigger(_controller.AttackHash);
            _controller.EntityDamager.StartAttack();
            _controller.AttackTimer.Reset();
            _controller.AttackTimer.Start();
        }
    }
}