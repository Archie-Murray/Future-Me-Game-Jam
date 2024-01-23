using UnityEngine;

namespace Enemy {
    public class EnemyIdleState : EnemyState {
        public EnemyIdleState(EnemyController controller, SpriteRenderer spriteRenderer, EnemyManager enemyManager, EnemyState.EnemyStateFactory enemyStateFactory) : base(controller, spriteRenderer, enemyManager, enemyStateFactory) { }

        public override void Start() { 
            _spriteRenderer.color = Color.gray; 
        }

        public override void FixedUpdate() { }
        public override void Update() { }
        public override void CheckTransitions() { }
        public override void Exit() { }
    }
}