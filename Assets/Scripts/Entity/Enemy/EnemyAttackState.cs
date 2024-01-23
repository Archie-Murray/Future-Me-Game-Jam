
using UnityEngine;

using Utilities;

namespace Enemy {
    public class EnemyAttackState : EnemyState {

        public EnemyAttackState(EnemyController controller, SpriteRenderer spriteRenderer, EnemyManager enemyManager, EnemyState.EnemyStateFactory enemyStateFactory) : base(controller, spriteRenderer, enemyManager, enemyStateFactory) { }

        public override void Start() {
            _spriteRenderer.color = Color.red;
        }
        public override void FixedUpdate() {
            if (_controller.AttackTimer.IsFinished) {
                CreateMissile();
            }
        }

        private void CreateMissile() {
            _controller.Emitter.Play(SoundEffectType.ENEMY_SHOOT);
            GameObject projectile = GameObject.Instantiate(_enemyManager.EnemyProjectile, _controller.transform.position + (2f * _controller.transform.up), _controller.transform.rotation);
            if (_controller) {
                _spriteRenderer.FlashColour(Color.magenta, 0.5f, _controller);
            }
            _controller.AttackTimer.Reset();
            _controller.AttackTimer.Start();
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
            _spriteRenderer.color = Color.white;
        }
    }
}