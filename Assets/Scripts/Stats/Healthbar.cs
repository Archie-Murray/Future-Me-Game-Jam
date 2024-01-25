using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    [Header("Must be set in editor!")]
    [SerializeField] private Health _health;
    [SerializeField] private TMP_Text _healthDisplay;
    [SerializeField] private float _animationSpeed = 5f;
    [Header("Debug")]
    [SerializeField] private float _currentValue;
    [SerializeField] private Slider _healthBar;

    private void Start() {
        if (!_health) {
            Debug.Log("No Health Component Assigned in editor");
            Destroy(this);
            return;
        }
        _currentValue = _health.PercentHealth;
        _healthBar = GetComponent<Slider>();
        _healthBar.value = _currentValue;
        _healthDisplay = GetComponentInChildren<TMP_Text>();
        _health.OnDeath += () => enabled = false;
    }

    private void FixedUpdate() {
        _currentValue = Mathf.MoveTowards(_currentValue, _health.PercentHealth, Time.fixedDeltaTime * _animationSpeed);
        _healthBar.value = _currentValue;
        _healthDisplay.text = _health.GetHealthText();
    }
}