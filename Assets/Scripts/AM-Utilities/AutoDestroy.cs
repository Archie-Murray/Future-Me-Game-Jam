using UnityEngine;

public class AutoDestroy : MonoBehaviour {
    public float Duration;

    private void Start() {
        Destroy(gameObject, Duration);
    }
}