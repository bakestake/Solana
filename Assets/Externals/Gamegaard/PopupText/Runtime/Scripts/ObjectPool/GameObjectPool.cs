using System;
using UnityEngine;

namespace Gamegaard.Pooling
{
    public class GameObjectPool<T> where T : MonoBehaviour
    {
        private readonly ObjectPool<T> pool;

        public int Count => pool.Count;

        public GameObjectPool(T prefab, Transform parent = null, int initialSize = 0, int maxSize = int.MaxValue, Action<T> onCreated = null)
        {
            pool = new ObjectPool<T>(
                factoryMethod: () =>
                {
                    T instance = UnityEngine.Object.Instantiate(prefab, parent);
                    instance.gameObject.SetActive(false);
                    onCreated?.Invoke(instance);
                    return instance;
                },
                onGet: instance =>
                {
                    instance.gameObject.SetActive(true);
                },
                onRelease: instance =>
                {
                    instance.transform.SetParent(parent, false);
                    instance.gameObject.SetActive(false);
                },
                onDestroy: instance =>
                {
                    UnityEngine.Object.Destroy(instance.gameObject);
                },
                initialSize: initialSize,
                maxSize: maxSize
            );
        }

        public T Get() => pool.Get();

        public void Release(T instance) => pool.Release(instance);
    }
}