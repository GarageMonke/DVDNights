using System;
using UnityEngine;

namespace CorePatterns.ObjectPooling
{
    [Serializable]
    public class PoolData<T>
    {
        [SerializeField] private string poolKey;
        [SerializeField] private T poolObject;
        [SerializeField] private int poolSize;
        
        public string PoolKey => poolKey;
        public T PoolObject => poolObject;
        public int PoolSize => poolSize;
    }
}