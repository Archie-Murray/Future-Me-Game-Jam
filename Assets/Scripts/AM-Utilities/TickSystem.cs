using UnityEngine;
using UnityEngine.Events;

public class TickSystem : MonoBehaviour {
    const float MIN_TICK_DELAY = 1f / 120f;
    [SerializeField] private float _tickDelay = 0.1f;
    [SerializeField] private float _currentTime = 0f;
    public UnityAction<float> TickLoop;

    /// <summary>
    /// Sets the tick rate of the TickSystem
    /// Note: This resets the tick timer back to 0!
    /// </summary>
    /// <param name="ticksPerSecond">Number of ticks to perform in a second</param>
    public void SetTickRate(float ticksPerSecond) {
        _tickDelay = Mathf.Min(MIN_TICK_DELAY, 1f / ticksPerSecond);
        _currentTime = 0f;
    }

    private void FixedUpdate() {
        _currentTime += Time.fixedDeltaTime;
        if (_currentTime >= _tickDelay) {
            TickLoop?.Invoke(_currentTime);
            _currentTime = 0f;
        }
    }
}