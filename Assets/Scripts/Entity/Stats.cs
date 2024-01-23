using System;

using UnityEngine;

[Serializable] public enum StatType { ATTACK_DAMAGE, ATTACK_SPEED, MOVE_SPEED, MAX_HEALTH, DEFENCE }
public class Stats : MonoBehaviour {
    [SerializeField] private float _attackDamage = 10f;
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _health = 100f;
    [SerializeField] private float _defence = 10f;

    public Action<StatType, float> OnStatChange;

    public void IncreaseStat(StatType type, float amount) {
        switch (type) {
            case StatType.ATTACK_DAMAGE:
                _attackDamage += amount;
                break;
            case StatType.ATTACK_SPEED:
                _attackSpeed += amount;
                break;
            case StatType.MOVE_SPEED:
                _moveSpeed += amount;
                break;
            case StatType.MAX_HEALTH:
                _health += amount;
                break;
            case StatType.DEFENCE:
                _defence += amount;
                break;
        }
        OnStatChange?.Invoke(type, amount);
    }

    public float GetStat(StatType type) {
        return type switch {
            StatType.ATTACK_DAMAGE => _attackDamage,
            StatType.ATTACK_SPEED => _attackSpeed,
            StatType.MOVE_SPEED => _moveSpeed,
            StatType.MAX_HEALTH => _health,
            StatType.DEFENCE => _defence,
            _ => 0f
        };
    }
}