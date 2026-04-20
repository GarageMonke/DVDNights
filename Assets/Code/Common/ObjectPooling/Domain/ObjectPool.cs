using System.Collections.Generic;
using UnityEngine;

namespace CorePatterns.ObjectPooling
{
    public class ObjectPool<T> where T : Behaviour
    {
        private readonly T _prefab;
        private readonly Queue<T> _pool;
        private readonly Transform _parent;

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            _pool = new Queue<T>();
            _parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                T instanced = Object.Instantiate(_prefab, _parent);
                instanced.gameObject.SetActive(false);
                _pool.Enqueue(instanced);
            }
        }

        public T GetObject()
        {
            T poolObject = _pool.Count > 0 ? _pool.Dequeue() : Object.Instantiate(_prefab, _parent);
            poolObject.gameObject.SetActive(true);
            return poolObject;
        }

        public void ReturnToPool(T objectToReturn)
        {
            if (_pool.Contains(objectToReturn))
            {
                return;
            }
            
            objectToReturn.gameObject.SetActive(false);
            objectToReturn.transform.SetParent(_parent);
            _pool.Enqueue(objectToReturn);
        }
    }
}