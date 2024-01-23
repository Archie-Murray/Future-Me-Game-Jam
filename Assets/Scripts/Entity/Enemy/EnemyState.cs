using System;
using System.Collections.Generic;


using UnityEngine;
namespace Enemy {
    public abstract class EnemyState : IEnemyState {
        protected readonly EnemyController _controller;

        protected readonly EnemyManager _enemyManager;
        protected readonly EnemyStateFactory _enemyStateFactory;

        public EnemyState(EnemyController controller, EnemyManager enemyManager, EnemyStateFactory enemyStateFactory) {
            _controller = controller;
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
                { typeof(EnemyIdleState), new EnemyIdleState(controller, manager, this) },
                { typeof(EnemyPatrolState), new EnemyPatrolState(controller, manager, this) },
                { typeof(EnemyChaseState), new EnemyChaseState(controller, manager, this) },
                { typeof(EnemyAttackState), new EnemyAttackState(controller, manager, this) },
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