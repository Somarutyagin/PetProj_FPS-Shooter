using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> pool = new Queue<T>();
    private GameObject prefab;
    private Transform parent;

    public ObjectPool(GameObject prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
        for (int i = 0; i < initialSize; i++)
        {
            AddObjectToPool();
        }
    }

    private T AddObjectToPool()
    {
        GameObject obj = Object.Instantiate(prefab, parent);
        obj.SetActive(false);
        T component = obj.GetComponent<T>();
        pool.Enqueue(component);
        return component;
    }

    public T Get()
    {
        T item;
        if (pool.Count > 0)
        {
            item = pool.Dequeue();
        }
        else
        {
            item = AddObjectToPool();
        }
        item.gameObject.SetActive(true);
        return item;
    }

    public void Return(T item)
    {
        item.gameObject.SetActive(false);

        pool.Enqueue(item);
    }

    public void Clear()
    {
        while (pool.Count > 0)
        {
            T item = pool.Dequeue();
            if (item != null) Object.Destroy(item.gameObject);
        }
    }
}
