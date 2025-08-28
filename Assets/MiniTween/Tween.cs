using System;
using UnityEngine;

namespace MiniTween
{
    public enum EasingType
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint
    }

    /// <summary>
    /// Classe générique pour gérer les animations de type "tweening".
    /// Permet d'interpoler entre une valeur de départ et une valeur de fin sur une durée donnée.
    /// </summary>
    public class Tween<T>
    {
        private Func<T> getter;
        private Action<T> setter;
        private T startValue;
        private T endValue;
        private float duration;
        private float elapsed;
        private bool isComplete;

        private Func<float, float> easeFunc;

        public Action OnComplete;

        public EasingType easing;

        public Tween(Func<T> getter, Action<T> setter, T endValue, float duration, EasingType easing = EasingType.Linear)
        {
            this.getter = getter;
            this.setter = setter;
            this.startValue = getter();
            this.endValue = endValue;
            this.duration = duration;
            this.easing = easing;
            Tween<T>.ApplyEasing(easing, out this.easeFunc);
        }

        public bool Update(float deltaTime)
        {
            if (isComplete) return true;

            elapsed += deltaTime;
            float t = Mathf.Min(elapsed / duration, 1f);
            t = easeFunc(t);

            if (typeof(T) == typeof(float))
            {
                float a = Convert.ToSingle(startValue);
                float b = Convert.ToSingle(endValue);
                float value = Mathf.Lerp(a, b, t);
                setter((T)(object)value);
            }
            else if (typeof(T) == typeof(Vector3))
            {
                Vector3 a = (Vector3)(object)startValue;
                Vector3 b = (Vector3)(object)endValue;
                Vector3 value = Vector3.Lerp(a, b, t);
                setter((T)(object)value);
            }

            if (elapsed >= duration)
            {
                isComplete = true;
                OnComplete?.Invoke();
            }

            return isComplete;
        }

        public static void ApplyEasing(EasingType type, out Func<float, float> easeFunc)
        {
            switch (type)
            {
                case EasingType.Linear:
                    easeFunc = t => t;
                    break;
                case EasingType.EaseInQuad:
                    easeFunc = t => t * t;
                    break;
                case EasingType.EaseOutQuad:
                    easeFunc = t => t * (2 - t);
                    break;
                case EasingType.EaseInOutQuad:
                    easeFunc = t => t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
                    break;
                case EasingType.EaseInCubic:
                    easeFunc = t => t * t * t;
                    break;
                case EasingType.EaseOutCubic:
                    easeFunc = t => (--t) * t * t + 1;
                    break;
                case EasingType.EaseInOutCubic:
                    easeFunc = t => t < 0.5f ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
                    break;
                case EasingType.EaseInQuart:
                    easeFunc = t => t * t * t * t;
                    break;
                case EasingType.EaseOutQuart:
                    easeFunc = t => 1 - (--t) * t * t * t;
                    break;
                case EasingType.EaseInOutQuart:
                    easeFunc = t => t < 0.5f ? 8 * t * t * t * t : 1 - 8 * (--t) * t * t * t;
                    break;
                case EasingType.EaseInQuint:
                    easeFunc = t => t * t * t * t * t;
                    break;
                case EasingType.EaseOutQuint:
                    easeFunc = t => 1 + (--t) * t * t * t * t;
                    break;
                case EasingType.EaseInOutQuint:
                    easeFunc = t => t < 0.5f ? 16 * t * t * t * t * t : 1 + 16 * (--t) * t * t * t * t;
                    break;
                default:
                    easeFunc = t => t;
                    break;
            }
        }

        public void Kill() => isComplete = true;
    }
}