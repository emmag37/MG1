using UnityEngine;
using System;


public class GameObjectPool<TObj, TData> where TObj : Component, ILinkable<TObj, TData>
{
    // ==================================================
    // Private Fields
    // ==================================================
    private TObj[] objects;

    private TObj head = null;
    private TObj tail = null;

    // ==================================================
    // Constructors
    // ==================================================
    public GameObjectPool(int size, GameObject prefab, TData data)  // optional params: maybe parent/other values for instantiate
    {
        if (prefab == null)
            throw new ArgumentNullException(nameof(prefab), "Object pool requires a valid prefab.");
        if (!prefab.TryGetComponent<TObj>(out _))
            throw new ArgumentException($"Prefab '{prefab.name}' has no {typeof(TObj)} component.", nameof(prefab));

        objects = new TObj[size];
        for (int i = 0; i < size; i++)
        {
            objects[i] = UnityEngine.Object.Instantiate(prefab).GetComponent<TObj>();
            objects[i].Initialize(data);
            AppendToFreeList(objects[i]);
        }
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public bool TryGetObject(out TObj obj)
    {
        obj = GetFreeObject();
        return obj != null;
    }

    public bool RemoveObject(TObj obj)
    {
        return AppendToFreeList(obj);
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private bool AppendToFreeList(TObj obj)
    {
        Debug.Assert(obj != null, "[GameObjectPool] Attempted to add a null object to free list");

        if (obj == head || obj.Next != null || obj == tail)
        {
            Debug.LogError("[GameObjectPool] Attempted a double free");
            return false;
        }

        if (head == null)
        {
            head = obj;
            obj.Prev = null;
        }
        else
        {
            tail.Next = obj;
            obj.Prev = tail;
        }

        obj.Next = null;
        tail = obj;
        obj.gameObject.SetActive(false);

        return true;
    }

    private TObj GetFreeObject()
    {
        if (head == null)
        {
            Debug.LogWarning($"[GameObjectPool] No available objects - all {objects.Length} objects in use");
            return null;
        }

        // removes and returns the head of the free list
        TObj obj = head;
        head = head.Next;

        if (head == null)  // no more pieces in the list
            tail = null;
        else
            head.Prev = null;

        obj.Next = null;
        obj.gameObject.SetActive(true);

        return obj;
    }
}
