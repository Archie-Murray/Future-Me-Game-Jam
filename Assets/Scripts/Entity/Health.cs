using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour {
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _defence;
    [SerializeField] private float _deathTimer = 0.25f;
    [SerializeField] private bool _isInvulnerable = false;
    [SerializeField] private Color _originalColour;

    public event UnityAction<float> OnDamage;
    public event UnityAction<float> OnHeal;
    public event UnityAction OnDeath;

    public float PercentHealth => Mathf.Clamp01(_currentHealth / _maxHealth);
    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;

    public void Init(float maxHealth, float defence) {
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
        _defence = defence;
    }

    public string GetHealthText() {
        return $"{_currentHealth:0} / {_maxHealth:0} ({PercentHealth:0%})";
    }

    public static float DefenceToMultiplier(float defence) {
        return defence < 0.0f ? 2.0f - (100.0f / (100.0f - defence)) : 100.0f / (100.0f + defence);
    }

    public void UpdateHealthAndDefence(StatType type, float amount) {
        if (type == StatType.MAX_HEALTH) {
            float diff = _maxHealth - amount;
            _maxHealth += amount;
            Heal(diff);
        } else if (type == StatType.DEFENCE) {
            _defence += amount;
        }
    }

    public void Damage(float amount) {
        if (_isInvulnerable) {
            return;
        }
        if (_currentHealth < 0f) {
            return;
        }
        amount *= DefenceToMultiplier(_defence);
        _currentHealth = Mathf.Max(_currentHealth - amount, 0f);
        OnDamage?.Invoke(amount);
        if (_currentHealth == 0f) {
            OnDeath?.Invoke();
            Destroy(gameObject, _deathTimer);
        }
    }

    public void Heal(float amount) { 
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        OnHeal?.Invoke(amount);
    }
}