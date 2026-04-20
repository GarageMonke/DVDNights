using UnityEngine;

namespace CorePatterns.Providers
{
    public abstract class IdentificableProvider<D,T> : Provider<T>
    {
        [SerializeField] private D identification;
        
        public D Identification => identification;
    }
}