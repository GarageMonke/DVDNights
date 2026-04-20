using System.Collections.Generic;
using UnityEngine;

namespace CorePatterns.Providers
{
    public abstract class ProviderHandler<D, T> : ScriptableObject
    {
        [SerializeField] List<IdentificableProvider<D,T>> providers;

        private Dictionary<D, Provider<T>> _providersDictionary;


        public void InitializeProviderHandler()
        {
            _providersDictionary = new Dictionary<D, Provider<T>>();

            for (int i = 0; i < providers.Count; i++)
            {
                _providersDictionary.Add(providers[i].Identification, providers[i]);
                providers[i].InitializeProvider();
            }
        }

        public T GetRandomElementById(D identification)
        {
            if (!_providersDictionary.TryGetValue(identification, out var provider))
            {
                return default;
            }

            return provider.GetRandomElement();
        }
    }
}