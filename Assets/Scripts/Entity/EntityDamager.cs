using System;
using System.Collections.Generic;

using UnityEngine;
public class EntityDamager : MonoBehaviour {
    [SerializeField] private List<GameObject> _hitObjects;
    [SerializeField] private bool _canHit = false;

    private void Awake() {
        _hitObjects = new List<GameObject>();
    }

    public void StartAttack() {
        _hitObjects.Clear();
        _canHit = true;
    }

    public void EndAttack() {
        _canHit = false;
    }

    public Action<Health, Vector3> OnHit;

    private void OnTriggerEnter(Collider other) {
        if (!_canHit) { return; }
        if (other.TryGetComponent(out Health hitHealth)) {
            OnHit?.Invoke(hitHealth, other.ClosestPoint(transform.position));
        }
    }
}