using System.Collections.Generic;
using UnityEngine;

namespace CorePatterns.Providers
{
    public abstract class Provider<T> : ScriptableObject, IProvider<T>
    {
        [SerializeField] private List<ProvidedElement<T>> assignedElements;
        
        private Dictionary<string, T> _elementsDictionary;
        
        public void InitializeProvider()
        {
            _elementsDictionary = new Dictionary<string, T>();

            foreach (ProvidedElement<T> providedElement in assignedElements)
            {
                _elementsDictionary.TryAdd(providedElement.Id, providedElement.Value);
            }
        }

        public T GetElementById(string elementId)
        {
            if (!_elementsDictionary.ContainsKey(elementId))
            {
                return default;
            }
            
            return _elementsDictionary[elementId];
        }
        
        public T GetRandomElement()
        {
            List<T> allElements = GetAllElements();
            T randomElement = allElements[Random.Range(0, allElements.Count)];
            return randomElement;
        }

        public List<T> GetAllElements()
        {
           List<T> elements = new List<T>();

           foreach (T element in _elementsDictionary.Values)
           {
               elements.Add(element);
           }
           
           return elements;
        }
    }
}