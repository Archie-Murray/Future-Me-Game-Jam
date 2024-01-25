using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

public static class Extensions {
    public static IEnumerable<Enum> GetFlags(this Enum flags) {
        return Enum.GetValues(flags.GetType()).Cast<Enum>().Where(flags.HasFlag);
    }

    public static List<DictKeyValue<TKey, TValue>> ToSerialisableList<TKey, TValue>(this Dictionary<TKey, TValue> dict) {
        return new List<DictKeyValue<TKey, TValue>>(
            dict.Keys.ToList().ConvertAll((TKey key) => new DictKeyValue<TKey, TValue>(key, dict[key]))
        );
    }
     
    /// <summary>
    /// Gets, or adds if doesn't contain a component
    /// </summary>
    /// <typeparam name="T">Component Type</typeparam>
    /// <param name="gameObject">GameObject to get component from</param>
    /// <returns>Component</returns>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component {
        T component = gameObject.GetComponent<T>();
        if (!component) {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }
    
    /// <summary>
    /// Returns true if a GameObject has a component of type T
    /// </summary>
    /// <typeparam name="T">Component Type</typeparam>
    /// <param name="gameObject">GameObject to check for component on</param>
    /// <returns>If the component is present</returns>
    public static bool HasComponent<T>(this GameObject gameObject) where T : Component { 
        return gameObject.GetComponent<T>() != null; 
    }

    /// <summary>
    /// Allows for use of null propogation on Unity Components as Unity uses
    /// null as 'marked for destroying', for example:
    /// <code>
    /// float value = GetComponent&lt;MagicType&gt;().OrNull&lt;MagicType&gt;()?.MagicFloatField ?? _defaultMagicFloatValue;
    /// </code>
    /// </summary>
    /// <typeparam name="T">Type of UnityObject to check for being actually bull</typeparam>
    /// <param name="obj">Object to check for null reference on</param>
    /// <returns>T or null if marked as null</returns>
    public static T OrNull<T>(this T obj) where T : UnityEngine.Object => obj ? obj : null;

    ///<summary>
    ///Normalises a Vector3 if its magnitude is larger than one
    ///</summary>
    ///<param name="vector">Vector to clamp</param>
    public static void ClampToNormalised(this Vector3 vector) {
        if (vector.magnitude > 1f) {
            vector.Normalize();
        }
    }

    ///<summary>
    ///Modifies the specified component(s) of a vector
    ///</summary>
    ///<param name="vector">Vector to modifiy</param>
    ///<param name="x">New x value if specified</param>
    ///<param name="y">New y value if specified</param>
    ///<param name="z">New z value if specified</param>
    ///<returns>Modified vector</returns>
    public static Vector3 With(this Vector3 vector, float? x, float? y, float? z) {
        return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
        
    }

    ///<summary>
    ///Adds to the specified component(s) of a vector
    ///</summary>
    ///<param name="vector">Vector to modifiy</param>
    ///<param name="x">Increase in x value if specified</param>
    ///<param name="y">Increase in y value if specified</param>
    ///<param name="z">Increase in z value if specified</param>
    ///<returns>Modified vector</returns>
    public static Vector3 Add(this Vector3 vector, float? x = 0, float? y = 0, float? z = 0) {
        return new Vector3(vector.x + (x ?? 0f), vector.y + (y ?? 0f), vector.z + (z ?? 0f));
    }

    ///<summary>
    ///Adds to the specified component(s) of a vector
    ///</summary>
    ///<param name="vector">Vector to modifiy</param>
    ///<param name="x">Increase in x value if specified</param>
    ///<param name="y">Increase in y value if specified</param>
    ///<param name="z">Increase in z value if specified</param>
    ///<returns>Modified vector</returns>
    public static Vector2 Multiply(this Vector2 vector, float? x = 1f, float? y = 1f) {
        return new Vector2(vector.x * (x ?? 1f), vector.y * (y ?? 1f));
    }

    public static Vector2 ClampMagnitude(this Vector2 vector, float magnitude) {
        if (vector.sqrMagnitude > (magnitude * magnitude)) {
            vector.Normalize();
            vector *= magnitude;
        }
        return vector;
    }

    ///<summary>
    ///Changes the colour of the material on the provided SpriteRenderer for the specified time
    ///using a coroutine that must have the MonoBehaviour to attach the coroutine to
    ///</summary>
    ///<param name="spriteRenderer">SpriteRenderer to change material colour of</param>
    ///<param name="colour">Colour to change SpriteRenderer material to</param>
    ///<param name="time">Time until colour changes back</param>
    ///<param name="monoBehaviour">MonoBehaviour to start coroutine on</param>
    public static void FlashColour(this SpriteRenderer spriteRenderer, Color colour, float time, MonoBehaviour monoBehaviour) {
        if (monoBehaviour.OrNull() == null) {
            Debug.Log("Provided MonoBehaviour was null!");
            return;
        }
        monoBehaviour.StartCoroutine(Flash(spriteRenderer, colour, time));
    }

    public static void FlashColour(this SpriteRenderer spriteRenderer, Color flashColour, Color originalColour, float time, MonoBehaviour monoBehaviour) {
        if (monoBehaviour.OrNull() == null) {
            Debug.Log("Provided MonoBehaviour was null!");
            return;
        }
        monoBehaviour.StartCoroutine(Flash(spriteRenderer, flashColour, originalColour, time));
    }


    private static IEnumerator Flash(SpriteRenderer spriteRenderer, Color colour, float time) { 
        Color original = spriteRenderer.color;
        spriteRenderer.color = colour;
        yield return Yielders.WaitForSeconds(time);
        spriteRenderer.color = original;
    }

    private static IEnumerator Flash(SpriteRenderer spriteRenderer, Color colour, Color originalColour, float time) { 
        spriteRenderer.color = colour;
        yield return Yielders.WaitForSeconds(time);
        spriteRenderer.color = originalColour;
    }

    public static void FadeCanvas(this CanvasGroup canvasGroup, float duration, bool fadeToTransparent, MonoBehaviour monoBehaviour) {
        monoBehaviour.StartCoroutine(CanvasFade(canvasGroup, duration, fadeToTransparent));
    }

    private static IEnumerator CanvasFade(CanvasGroup canvasGroup, float duration, bool fadeToTransparent) {
        CountDownTimer timer = new CountDownTimer(duration);
        timer.Start();
        while (timer.IsRunning) {
            canvasGroup.alpha = fadeToTransparent ? timer.Progress : 1f - timer.Progress;
            timer.Update(Time.fixedDeltaTime);
            yield return Yielders.WaitForFixedUpdate;
        }
        if (fadeToTransparent) {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0f;
        } else {
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1f;
        }
    }

    [Serializable]
    public struct DictKeyValue<TKey, TValue> {
        public TKey Key;
        public TValue Value;

        public DictKeyValue(TKey key, TValue value) {
            Key = key;
            Value = value;
        }
    }
}
