using System;
using System.Collections.Generic;


using UnityEngine;
namespace Enemy {
    public abstract class EnemyState : IEnemyState {
        protected readonly EnemyController _controller;
        protected readonly SpriteRenderer _spriteRenderer;
        protected readonly EnemyManager _enemyManager;
        protected readonly EnemyStateFactory _enemyStateFactory;

        public EnemyState(EnemyController controller, SpriteRenderer spriteRenderer, EnemyManager enemyManager, EnemyStateFactory enemyStateFactory) {
            _controller = controller;
            _spriteRenderer = spriteRenderer;
            _enemyManager = enemyManager;
            _enemyStateFactory = enemyStateFactory;
        }

        public abstract void Start();
        public abstract void FixedUpdate();
        public abstract void Update();
        public abstract void CheckTransitions();
        public abstract void Exit();

        public void SwitchState(EnemyState state) {
            _controller.State?.Exit();
            _controller.State = state;
            _controller.State.Start();
        }

        public class EnemyStateFactory {

            private Dictionary<Type, EnemyState> _states;

            public EnemyStateFactory(EnemyController controller, SpriteRenderer renderer, EnemyManager manager) {
                _states = new Dictionary<Type, EnemyState>() {
                { typeof(EnemyIdleState), new EnemyIdleState(controller, renderer, manager, this) },
                { typeof(EnemyPatrolState), new EnemyPatrolState(controller, renderer, manager, this) },
                { typeof(EnemyChaseState), new EnemyChaseState(controller, renderer, manager, this) },
                { typeof(EnemyAttackState), new EnemyAttackState(controller, renderer, manager, this) },
            };
            }

            public void ValidateStates() {
                foreach (EnemyState state in _states.Values) {
                    Debug.Log($"{state.GetType()} is valid: {(state._enemyStateFactory == null ? "no" : "yes")}");
                }
            }

            public T State<T>() where T : EnemyState {
                if (!_states.ContainsKey(typeof(T))) {
                    Debug.Log($"Could not find state: {typeof(T)}");
                    return _states[typeof(EnemyIdleState)] as T;
                }
                return _states[typeof(T)] as T;
            }
        }
    }
}