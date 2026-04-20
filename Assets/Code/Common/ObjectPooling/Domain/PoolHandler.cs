using System.Collections.Generic;
using UnityEngine;

namespace CorePatterns.ObjectPooling
{
    public class PoolHandler<T> : IPoolHandler<T> where T : Behaviour
    {
        private readonly Dictionary<string, ObjectPool<T>> _pools = new();
        private readonly Transform _parent;

        public PoolHandler(Transform parent = null)
        {
            _parent = parent;
        }

        public void CreatePool(string key, T prefab, int initialSize)
        {
            if (!_pools.ContainsKey(key))
            {
                _pools[key] = new ObjectPool<T>(prefab, initialSize, _parent);
            }
        }

        public T GetObject(string key)
        {
            if (_pools.TryGetValue(key, out ObjectPool<T> pool))
            {
                return pool.GetObject();
            }

            Debug.LogError($"No pool found for key: {key}");
            return null;
        }

        public void ReturnToPool(string key, T instance)
        {
            if (_pools.TryGetValue(key, out ObjectPool<T> pool))
            {
                pool.ReturnToPool(instance);
                return;
            }

            Debug.LogError($"No pool found for key: {key}");
        }
    }
}