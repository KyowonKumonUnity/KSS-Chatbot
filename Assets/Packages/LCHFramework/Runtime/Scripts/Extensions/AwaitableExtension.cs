using System;
using UnityEngine;

namespace LCHFramework.Extensions
{
    public static class AwaitableExtension
    {
        public static async void Forget(this Awaitable awaitable, bool logException = true)
        {
            try
            {
                await awaitable;
            }
            catch (Exception e)
            {
                if (logException) UnityEngine.Debug.LogException(e);
            }
        }
        
        public static async void Forget<T>(this Awaitable<T> awaitable, bool logException = true)
        {
            try
            {
                await awaitable;
            }
            catch (Exception e)
            {
                if (logException) UnityEngine.Debug.LogException(e);
            }
        }
        
        public static async Awaitable SuppressCancellationThrow(this Awaitable awaitable)
        {
            try
            {
                await awaitable;
            }
            catch (OperationCanceledException)
            {
            }
        }
        
        public static async Awaitable<T> SuppressCancellationThrow<T>(this Awaitable<T> awaitable)
        {
            try
            {
                await awaitable;
            }
            catch (OperationCanceledException)
            {
            }
            
            return default;
        }
    }
}