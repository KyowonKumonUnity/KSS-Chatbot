using System;

namespace LCHFramework.Utilities
{
    public static class IDisposableUtility
    {
        public static void DisposeAndSetDefault<T>(ref T disposable) where T : IDisposable
        {
            disposable?.Dispose();
            disposable = default;
        }
    }
}