using System;

namespace CorePatterns.Providers
{
    [Serializable]
    public class ProvidedElement<T>
    {
        public string Id;
        public T Value;
    }
}