using UnityEngine;

namespace MiniTween
{
    public static class TransformExtensions
    {
        // =========================
        // DO MOVE
        // =========================
        public static Tween<Vector3> DoMove(this Transform t, Vector3 target, float duration, EasingType easing = EasingType.Linear)
        {
            var tween = new Tween<Vector3>(
                () => t.position,
                v => t.position = v,
                target,
                duration,
                easing
            );
            TweenManager.Add(tween);
            return tween;
        }

        // =========================
        // DO SCALE
        // =========================
        public static Tween<Vector3> DoScale(this Transform t, Vector3 target, float duration, EasingType easing = EasingType.Linear)
        {
            var tween = new Tween<Vector3>(
                () => t.localScale,
                v => t.localScale = v,
                target,
                duration,
                easing
            );
            TweenManager.Add(tween);
            return tween;
        }

        // Shortcut : scale uniforme
        public static Tween<Vector3> DoScale(this Transform t, float uniformScale, float duration, EasingType easing = EasingType.Linear)
        {
            return DoScale(t, Vector3.one * uniformScale, duration, easing);
        }

        // =========================
        // DO ROTATE
        // =========================
        public static Tween<Vector3> DoRotate(this Transform t, Vector3 targetEuler, float duration, EasingType easing = EasingType.Linear)
        {
            var tween = new Tween<Vector3>(
                () => t.eulerAngles,
                v => t.eulerAngles = v,
                targetEuler,
                duration,
                easing
            );
            TweenManager.Add(tween);
            return tween;
        }

        // =========================
        // DO SHAKE
        // =========================
        public static Tween<float> DoShake(this Transform t, float duration, float magnitude, EasingType easing = EasingType.Linear)
        {
            Vector3 originalPos = t.localPosition;

            var tween = new Tween<float>(
                () => 0f,
                progress =>
                {
                    Vector3 rand = new Vector3(
                        Random.Range(-1f, 1f),
                        Random.Range(-1f, 1f),
                        0
                    ) * magnitude * (1f - progress);

                    t.localPosition = originalPos + rand;
                },
                1f,
                duration,
                easing
            );

            tween.OnComplete = () => t.localPosition = originalPos;
            TweenManager.Add(tween);
            return tween;
        }
    }
}