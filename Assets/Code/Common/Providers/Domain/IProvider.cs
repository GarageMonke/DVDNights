using System.Collections.Generic;

namespace CorePatterns.Providers
{
    public interface IProvider<T>
    {
        public void InitializeProvider();
        public T GetElementById(string elementId);
        public List<T> GetAllElements();
    }
}