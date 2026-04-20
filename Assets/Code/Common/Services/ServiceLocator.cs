using System;
using System.Collections.Generic;

namespace CorePatterns.ServiceLocator
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void RegisterService<TServiceType>(TServiceType service)
        {
            Type type = typeof(TServiceType);

            _services.Add(type, service);
        }

        public static TServiceType GetService<TServiceType>()
        {
            Type type = typeof(TServiceType);

            if (!_services.TryGetValue(type, out var service))
                return default;

            return (TServiceType)service;
        }

        public static void RemoveService<TServiceType>()
        {
            Type type = typeof(TServiceType);

            if (!_services.ContainsKey(type))
                throw new Exception($"The service {type.Name} is not in the service locator. So you can't remove.");

            _services.Remove(type);
        }

        public static bool IsServiceRegistered<TServiceType>()
        {
            Type type = typeof(TServiceType);

            return _services.ContainsKey(type);
        }
    }
}