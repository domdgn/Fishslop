using UnityEngine;

namespace DebugPanel.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Tries to get component. If it does not exist, it gets added.
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            bool found = gameObject.TryGetComponent(out T component);

            return found ? component : gameObject.AddComponent<T>();
        }
    }
}