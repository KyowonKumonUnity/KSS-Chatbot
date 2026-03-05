using System;
using System.Collections.Generic;
using UnityEngine;

namespace LCHFramework.Extensions
{
    public static class TransformExtension
    {
        public static Transform[] GetChildren(this Transform transform, bool includeInactive = false, int depth = 1)
        {
            if (depth < 0) return Array.Empty<Transform>();
            if (depth == 0 && !includeInactive && !transform.gameObject.activeSelf) return Array.Empty<Transform>();
            if (depth == 0 && (includeInactive || transform.gameObject.activeSelf)) return new[] { transform };

            var queue = new Queue<(Transform, int)>();
            queue.Enqueue((transform, 0));
            var result = new List<Transform>();
            while (0 < queue.Count)
            {
                var (current, currentDepth) = queue.Dequeue();

                if (currentDepth == depth)
                {
                    if (includeInactive || current.gameObject.activeSelf) result.Add(current);
                }
                else
                    foreach (Transform child in current) queue.Enqueue((child, currentDepth + 1));
            }
            return result.ToArray();
        }
        
        public static Transform[] GetSiblings(this Transform transform, bool includeInactive = false)
        {
            var result = new List<Transform>();
            for (var i = 0; transform.parent != null && i < transform.parent.childCount; i++)
            {
                var sibling = transform.parent.GetChild(i);
                if (includeInactive || sibling.gameObject.activeSelf) result.Add(sibling);
            }
            return result.ToArray();
        }
        
        public static int GetDepth(this Transform from, Transform to = null)
        {
            var result = 1;
            do
            {
                if (from.parent != null && from.parent != to)
                {
                    result++;
                    from = from.parent;
                }
                else if (from.parent == to)
                    return result;
                else
                    return -1;
                    
            } while (true);
        }
        
        public static Transform FindInChildren(this Transform transform, string n)
            => transform.FindInChildren(childName => n == childName);
        
        public static Transform FindInChildren(this Transform transform, Func<string, bool> predicate)
        {
            var stack = new Stack<Transform>();
            var parent = transform;
            while (true)
            {
                for (var i = parent.childCount - 1; 0 <= i; i--) stack.Push(parent.GetChild(i));
                if (0 < stack.Count)
                {
                    parent = stack.Pop();
                    if (predicate(parent.name)) return parent;
                }
                else return null;
            }
        }
    }
}