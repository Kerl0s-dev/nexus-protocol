using System.Collections.Generic;
using UnityEngine;

namespace MiniTween
{
    /// <summary>
    /// Gestionnaire global pour les tweens.
    /// Permet d'ajouter et de mettre à jour tous les tweens actifs.
    /// </summary>
    public static class TweenManager
    {
        private static List<object> tweens = new List<object>();

        public static void Add<T>(Tween<T> tween) => tweens.Add(tween);

        public static void Update(float deltaTime)
        {
            for (int i = tweens.Count - 1; i >= 0; i--)
            {
                var tweenObj = tweens[i];
                bool done = false;

                if (tweenObj is Tween<float> tf) done = tf.Update(deltaTime);
                else if (tweenObj is Tween<Vector3> tv) done = tv.Update(deltaTime);

                if (done) tweens.RemoveAt(i);
            }
        }
    }
}