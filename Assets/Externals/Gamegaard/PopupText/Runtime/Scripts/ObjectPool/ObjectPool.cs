using System;
using System.Collections.Generic;

namespace Gamegaard.Pooling
{
    public class ObjectPool<T>
    {
        private readonly Stack<T> pool;
        private readonly Func<T> factoryMethod;
        private readonly Action<T> onGet;
        private readonly Action<T> onRelease;
        private readonly Action<T> onDestroy;
        private readonly int maxSize;

        public int Count => pool.Count;
        public int MaxSize => maxSize;

        public ObjectPool(Func<T> factoryMethod, Action<T> onGet = null, Action<T> onRelease = null, Action<T> onDestroy = null, int initialSize = 0, int maxSize = int.MaxValue)
        {
            this.factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));
            this.onGet = onGet;
            this.onRelease = onRelease;
            this.onDestroy = onDestroy;
            this.maxSize = maxSize;

            pool = new Stack<T>(Math.Max(initialSize, 4));
            Prewarm(initialSize);
        }

        private void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T instance = factoryMethod();
                pool.Push(instance);
            }
        }

        public T Get()
        {
            T item = pool.Count > 0 ? pool.Pop() : factoryMethod();
            onGet?.Invoke(item);

            if (item is IPoolable poolable)
                poolable.OnTakenFromPool();

            return item;
        }

        public void Release(T item)
        {
            if (item == null) return;

            onRelease?.Invoke(item);

            if (item is IPoolable poolable)
                poolable.OnReturnedToPool();

            if (pool.Count < maxSize)
            {
                pool.Push(item);
            }
            else
            {
                onDestroy?.Invoke(item);
            }
        }
    }
}