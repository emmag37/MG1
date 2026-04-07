using UnityEngine;
using System;
using System.Collections.Generic;

// need to define <, > , == for my leaderboard item

public abstract class CappedRankedList<T> where T : IComparable<T>
{
    // properties
    protected readonly int max;
    private static readonly IComparer<T> DescComparer =
        Comparer<T>.Create((a, b) => b.CompareTo(a));

    public IReadOnlyList<T> ROList => list;

    // serialized fields
    [SerializeField] protected List<T> list = new List<T>();

    protected CappedRankedList(int max)
    {
        this.max = max;
    }

    public virtual bool TryAddValue(T value)
    {
        if (list.Count == max && value.CompareTo(list[max - 1]) <= 0)   // less than or equal : -n means less than, 0 means equal
            return false;

        if (list.Count == max)
            list.RemoveAt(max - 1);

        InsertValue(value);
        return true;
    }

    private void InsertValue(T value)
    {
        // use built-in binary search with descending comparer
        int index = list.BinarySearch(value, DescComparer);

        // BinarySearch returns bitwise complement of insertion index to keep the list sorted
        if (index < 0)
            index = ~index;

        list.Insert(index, value);
    }
}


// Capped Ranked list of size 10

[Serializable]
public class ScoreHistory : CappedRankedList<int>
{
    public ScoreHistory() : base(10) { }
}

