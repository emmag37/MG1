using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class KeyedDatabase<TKey, TValue> : ScriptableObject
{
    // protected properties
    [Serializable]
    protected struct Entry
    {
        public TKey key;
        public TValue value;
    }

    // private fields
    [SerializeField] private Entry[] entries;
    private Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

    // initialization
    public void Initialize()
    {
        if (entries == null)
            throw new InvalidOperationException("[KeyedDatabase] Requires non-null entries array for initialization");

        foreach (Entry entry in entries)
        {
            if (dict.ContainsKey(entry.key))
            {
                Debug.LogError($"[KeyedDatabase] Duplicate key: {entry.key}");
                continue;
            }
            dict[entry.key] = entry.value;
        }
    }

    // access
    public bool TryGetValue(TKey key, out TValue value) => dict.TryGetValue(key, out value);
}