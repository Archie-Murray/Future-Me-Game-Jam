using UnityEngine;
public class Helpers : MonoBehaviour {
    public static Helpers I;

    private void Awake() {
        if (I == null) {
            I = this;
        } else {
            Destroy(this);
        }
        MainCamera = Camera.main;
    }

    public Camera MainCamera;
    public static Vector2 FromRadians(float radians) {
        return new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
    }
}
