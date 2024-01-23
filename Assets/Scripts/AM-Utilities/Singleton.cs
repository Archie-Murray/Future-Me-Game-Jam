using UnityEngine;
using System.Reflection;

public class Singleton<T> : MonoBehaviour where T : Component {
    protected static T _instance;
    public static bool HasInstance => _instance != null;
    public static T TryGetInstance() => HasInstance ? _instance : null;
    public static T Current => _instance;

    public static T Instance {
        get {
            if (_instance == null) {
                _instance = FindFirstObjectByType<T>();
                if (_instance == null) {
                    GameObject obj = new GameObject();
                    obj.name = $"{typeof(T).Name} - AutoCreated";
                    _instance = obj.AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake() => InitialiseSingleton();

    protected virtual void InitialiseSingleton() {
        if (!Application.isPlaying) {
            return;
        }
        _instance = this as T;
    }
}

public class PersistentSingleton<T> : Singleton<T> where T : Component {
    
    public static new T Instance {
        get {
            if (_instance == null) {
                _instance = FindFirstObjectByType<T>();
                if (_instance == null) {
                    GameObject obj = new GameObject() {
                        name = $"{typeof(T).Name} - AutoCreated"
                    };
                    _instance = obj.AddComponent<T>();
                    DontDestroyOnLoad(obj);
                }
            }

            return _instance;
        }
    }

    protected override void InitialiseSingleton() {
        if (!Application.isPlaying) {
            return;
        }
        DontDestroyOnLoad(gameObject);
        _instance = this as T;
    }
}