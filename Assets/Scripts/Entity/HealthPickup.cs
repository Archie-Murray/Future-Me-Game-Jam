using UnityEngine;

public class HealthPickup : MonoBehaviour {
    [SerializeField] private float _heal;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.HasComponent<PlayerController>()) {
            if (other.TryGetComponent(out Health playerHealth)) {
                playerHealth.Heal(_heal);
                Destroy(gameObject);
            }
        }
    }
}