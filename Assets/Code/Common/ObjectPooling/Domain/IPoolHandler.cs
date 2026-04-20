namespace CorePatterns.ObjectPooling
{
    public interface IPoolHandler<T>
    {
        public void CreatePool(string key, T prefab, int initialSize);
        public T GetObject(string key);
        public void ReturnToPool(string key, T instance);
    }
}