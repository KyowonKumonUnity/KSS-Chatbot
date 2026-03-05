using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LCHFramework.Extensions
{
    public static class GameObjectExtension
    {
        public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T result)
            => gameObject.TryGetComponentInParent(false, out result);
        
        public static bool TryGetComponentInParent<T>(this GameObject gameObject, bool includeInactive, out T result)
            => !EqualityComparer<T>.Default.Equals(result = gameObject.GetComponentInParent<T>(includeInactive), default);

        public static bool TryGetComponentsInParent<T>(this GameObject gameObject, out T[] result)
            => gameObject.TryGetComponentsInParent(false, out result);
        
        public static bool TryGetComponentsInParent<T>(this GameObject gameObject, bool includeInactive, out T[] result)
            => (result = gameObject.GetComponentsInParent<T>(includeInactive)).Any();

        public static bool TryGetComponentInSibling<T>(this GameObject gameObject, out T result)
            => gameObject.TryGetComponentInSibling(false, out result);
        
        public static bool TryGetComponentInSibling<T>(this GameObject gameObject, bool includeInactive, out T result)
            => !EqualityComparer<T>.Default.Equals(result = gameObject.GetComponentInSibling<T>(includeInactive), default);
        
        public static T GetComponentInSibling<T>(this GameObject gameObject, bool includeInactive = false)
        {
            foreach (var sibling in gameObject.transform.GetSiblings(includeInactive))
                if (sibling.TryGetComponent<T>(out var result)) return result;
            
            return default;
        }
        
        public static bool TryGetComponentsInSibling<T>(this GameObject gameObject, out T[] result)
            => gameObject.TryGetComponentsInSibling(false, out result);
        
        public static bool TryGetComponentsInSibling<T>(this GameObject gameObject, bool includeInactive, out T[] result)
            => (result = gameObject.GetComponentsInSibling<T>(includeInactive)).Any();
        
        public static T[] GetComponentsInSibling<T>(this GameObject gameObject, bool includeInactive = false)
        {
            var result = new List<T>();
            foreach (var sibling in gameObject.transform.GetSiblings(includeInactive))
                if (sibling.TryGetComponent<T>(out var t)) result.Add(t);
            
            return result.ToArray();
        }
        
        public static bool TryGetComponents<T>(this GameObject gameObject, out T[] result)
            => (result = gameObject.GetComponents<T>()).Any();
        
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
            => !gameObject.TryGetComponent<T>(out var result) ? gameObject.AddComponent<T>() : result;
        
        public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T result)
            => gameObject.TryGetComponentInChildren(false, out result);
        
        public static bool TryGetComponentInChildren<T>(this GameObject gameObject, bool includeInactive, out T result)
            => !EqualityComparer<T>.Default.Equals(result = gameObject.GetComponentInChildren<T>(includeInactive), default);

        public static bool TryGetComponentsInChildren<T>(this GameObject gameObject, out T[] result)
            => gameObject.TryGetComponentsInChildren(false, out result);
        
        public static bool TryGetComponentsInChildren<T>(this GameObject gameObject, bool includeInactive, out T[] result)
            => (result = gameObject.GetComponentsInChildren<T>(includeInactive)).Any();
    }
}