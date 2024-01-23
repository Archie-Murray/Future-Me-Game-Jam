using System.IO;
using System;

using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
public class Screenshot : MonoBehaviour {
    [SerializeField] private string _path = Path.DirectorySeparatorChar.ToString();
    [SerializeField] private KeyCode _key;

    private void Start() {
        if (_key == KeyCode.None) {
            _key = KeyCode.X;
        }
    }

    public void Update() {
        if (Input.GetKeyDown(_key)) {
            TakeScreenshot();
        }
    }

    public void TakeScreenshot() {
        ScreenCapture.CaptureScreenshot($"{Application.dataPath}{_path}Screenshot: {DateTime.Now.ToFileTime()}.png");
    } 
}
#endif