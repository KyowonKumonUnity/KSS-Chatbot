using System;
using System.Text;
using UnityEngine;

namespace LCHFramework.Extensions
{
    public static class ComponentExtension
    {
        public static bool TryGetComponentInParent<T>(this Component component, out T result)
            => component.TryGetComponentInParent(false, out result);

        public static bool TryGetComponentInParent<T>(this Component component, bool includeInactive, out T result)
            => component.gameObject.TryGetComponentInParent(includeInactive, out result);

        public static bool TryGetComponentsInParent<T>(this Component component, out T[] result)
            => component.TryGetComponentsInParent(false, out result);
        
        public static bool TryGetComponentsInParent<T>(this Component component, bool includeInactive, out T[] result)
            => component.gameObject.TryGetComponentsInParent(includeInactive, out result);

        public static bool TryGetComponentInSibling<T>(this Component component, out T result)
            => component.TryGetComponentInSibling(false, out result);
        
        public static bool TryGetComponentInSibling<T>(this Component component, bool includeInactive, out T result)
            => component.gameObject.TryGetComponentInSibling(includeInactive, out result);

        public static T GetComponentInSibling<T>(this Component component, bool includeInactive = false)
            => component.gameObject.GetComponentInSibling<T>(includeInactive);

        public static bool TryGetComponentsInSibling<T>(this Component component, out T[] result)
            => component.TryGetComponentsInSibling(false, out result);
        
        public static bool TryGetComponentsInSibling<T>(this Component component, bool includeInactive, out T[] result)
            => component.gameObject.TryGetComponentsInSibling(includeInactive, out result);

        public static T[] GetComponentsInSibling<T>(this Component component, bool includeInactive = false)
            => component.gameObject.GetComponentsInSibling<T>(includeInactive);

        public static bool TryGetComponents<T>(this Component component, out T[] result)
            => component.gameObject.TryGetComponents(out result);
        
        public static bool TryGetComponentInChildren<T>(this Component component, out T result)
            => component.TryGetComponentInChildren(false, out result);
        
        public static bool TryGetComponentInChildren<T>(this Component component, bool includeInactive, out T result)
            => component.gameObject.TryGetComponentInChildren(includeInactive, out result);

        public static bool TryGetComponentsInChildren<T>(this Component component, out T[] result)
            => component.TryGetComponentsInChildren(false, out result);
        
        public static bool TryGetComponentsInChildren<T>(this Component component, bool includeInactive, out T[] result)
            => component.gameObject.TryGetComponentsInChildren(includeInactive, out result); 
        
        public static void RadioActive(this Component component, bool value)
            => component.ActionInSiblings<Transform>(sibling => sibling.SetActive(sibling == component.transform ? value : !value));

        public static void ActionInSiblings<T>(this Component component, Action<T> action)
        {
            var parentOrNull = component.transform.parent;
            if (parentOrNull == null) return;
            
            for (var i = 0; i < parentOrNull.childCount; i++)
            {
                var type = parentOrNull.GetChild(i).GetComponent<T>();
                if (type != null) action?.Invoke(type);
            }
        }
        
        public static void SetActive(this Component component, bool value) 
            => component.gameObject.SetActive(value);
        
        public static string GetPath(this Component component)
        {
            var transform = component.transform;
            var path = new StringBuilder(transform.name);
            while ((transform = transform.parent) != null) path = path.Insert(0, $"{transform.name}/");

            return $"{component.gameObject.scene.name}/{path}";
        }
    }
}