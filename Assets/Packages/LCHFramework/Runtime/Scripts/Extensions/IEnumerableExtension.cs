using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace LCHFramework.Extensions
{
    public static class IEnumerableExtension
    {
        private static readonly Random Random = new();
        public static T Pick<T>(this IEnumerable<T> enumerable)
            => enumerable.ElementAt(Random.Next(enumerable.Count()));
        
        public static bool TryFirstOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate, out T result)
            => !EqualityComparer<T>.Default.Equals(result = enumerable.FirstOrDefault(predicate), default);
        
        public static bool TryLastOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate, out T result)
            => !EqualityComparer<T>.Default.Equals(result = enumerable.LastOrDefault(predicate), default);
        
        public static bool Exists<T>(this IEnumerable<T> enumerable)
            => !enumerable.IsEmpty();
        
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
            => enumerable == null || !enumerable.Any();
        
        public static bool TryIndexOf<T>(this IEnumerable<T> enumerable, T value, out int result)
            => -1 < (result = enumerable.IndexOf(value));
        
        public static int IndexOf<T>(this IEnumerable<T> enumerable, T value)
            => IndexOf(enumerable, t => EqualityComparer<T>.Default.Equals(t, value));
        
        public static int IndexOf<T>(this IEnumerable<T> enumerable, Func<T, bool> func)
        {
            var i = 0;
            foreach (var item in enumerable)
            {
                if (func.Invoke(item)) return i;
                i++;
            }
            return -1;
        }
        
        public static T ClampedElementAt<T>(this IEnumerable<T> enumerable, int index)
            => enumerable.ElementAt(Math.Clamp(index, 0, enumerable.Count() - 1));
        
        public static T ClampedElementAt<T>(this IEnumerable<T> enumerable, int index, int min, int max) 
            => enumerable.ElementAt(Math.Clamp(index, min, max));
        
        public static void RadioActive<T>(this IEnumerable<T> components, T item, bool value) where T : Component
            => components.Select(t => t.gameObject).RadioActive(item.gameObject, value);
        
        public static void RadioActive(this IEnumerable<GameObject> gameObjects, GameObject item, bool value) 
        {
            foreach (var t in gameObjects.Where(t => t != null)) t.SetActive(t == item ? value : !value);
        }
        
        public static void SetActive<T>(this IEnumerable<T> components, bool value) where T : Component
        {
            foreach (var component in components.Where(t => t != null)) component.SetActive(value);
        }
        
        public static void SetActive(this IEnumerable<GameObject> gameObjects, bool value)
        {
            foreach (var gameObject in gameObjects) gameObject.SetActive(value);
        }
        
        public static int FindIndex<T>(this IEnumerable<T> enumerable, Predicate<T> match)
        {
            var i = 0;
            var result = -1;
            foreach (var t in enumerable)
            {
                if (!match.Invoke(t))
                    i++;
                else
                {
                    result = i;
                    break;
                }
            }
            return result;
        }
        
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var t in enumerable) action?.Invoke(t);
        }
        
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
            var i = 0;
            foreach (var t in enumerable) action?.Invoke(t, i++);
        }
        
        public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
        {
            foreach (var t in enumerable) if (action != null) await action.Invoke(t);
        }
        
        public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, int, Task> action)
        {
            var i = 0;
            foreach (var t in enumerable) if (action != null) await action.Invoke(t, i++);
        }
    }
}