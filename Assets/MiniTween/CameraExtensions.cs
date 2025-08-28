using UnityEngine;

namespace MiniTween.CameraTween
{
    public enum ShakeType
    {
        Explosion,
        Damage
    }

    public enum TiltType
    {
        StrafeLeft,
        StrafeRight,
        Explosion
    }

    public static class CameraExtensions
    {
        // =========================
        // DO FOV
        // =========================
        public static Tween<float> DoFOV(this Camera cam, float targetFOV, float duration, EasingType easing = EasingType.Linear)
        {
            var tween = new Tween<float>(
                () => cam.fieldOfView,
                v => cam.fieldOfView = v,
                targetFOV,
                duration,
                easing
            );

            TweenManager.Add(tween);
            return tween;
        }

        // =========================
        // DO SHAKE
        // =========================

        // Champ pour stocker l'offset de shake
        private static Vector3 shakeOffset;

        public static Tween<float> DoShake(this Camera cam, float duration, float magnitude, EasingType easing = EasingType.Linear)
        {
            var tween = new Tween<float>(
                () => 0f,
                progress =>
                {
                    shakeOffset = new Vector3(
                        Random.Range(-1f, 1f),
                        Random.Range(-1f, 1f),
                        0
                    ) * magnitude * (1f - progress);
                },
                1f,
                duration,
                easing
            );

            tween.OnComplete = () => shakeOffset = Vector3.zero;
            TweenManager.Add(tween);
            return tween;
        }

        public static Tween<float> DoShake(this Camera cam, ShakeType type, EasingType easing = EasingType.Linear)
        {
            switch (type)
            {
                case ShakeType.Explosion:
                    return cam.DoShake(0.5f, .75f, easing);
                case ShakeType.Damage:
                    return cam.DoShake(0.2f, 0.05f, easing);
                default:
                    return null;
            }
        }

        // Méthode à appeler depuis LateUpdate
        public static Vector3 GetShakeOffset() => shakeOffset;
    }
}