using System;
using System.Collections.Generic;

namespace Game.Core.DI
{
    public static class DI
    {
        private static readonly Dictionary<Type, object> _map = new();

        public static void Bind<T>(T instance) where T : class
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            _map[typeof(T)] = instance;
        }

        public static T Resolve<T>() where T : class
        {
            if (_map.TryGetValue(typeof(T), out var obj))
                return (T)obj;
            throw new InvalidOperationException($"[DI] No binding for type {typeof(T).Name}");
        }

        public static bool TryResolve<T>(out T service) where T : class
        {
            if (_map.TryGetValue(typeof(T), out var obj))
            {
                service = (T)obj;
                return true;
            }
            service = null;
            return false;
        }

        public static void Clear() => _map.Clear();
    }
}