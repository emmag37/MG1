using UnityEngine;
using System;
using System.Collections.Generic;

// need to define <, > , == for my leaderboard item

public abstract class CappedRankedList<T> where T : IComparable<T>
{
    // properties
    protected readonly int max = 100;
    private static readonly IComparer<T> DescComparer =
        Comparer<T>.Create((a, b) => b.CompareTo(a));

    public IReadOnlyList<T> ROList => list;

    // serialized fields
    [SerializeField] protected List<T> list = new List<T>();

    // constructor
    protected CappedRankedList(int max)
    {
        this.max = max;
    }

    // public functions
    // returns index of inserted value, -1 if not inserted.
    public virtual bool TryAddValue(T value)
    {
        if (list.Count == max && value.CompareTo(list[max - 1]) <= 0)   // less than or equal : -n means less than, 0 means equal
            return false;

        if (list.Count == max)
            list.RemoveAt(max - 1);

        InsertValue(value);
        return true;
    }

    // private functions
    private int InsertValue(T value)
    {
        // use built-in binary search with descending comparer
        int index = list.BinarySearch(value, DescComparer);

        // BinarySearch returns bitwise complement of insertion index to keep the list sorted
        if (index < 0)
            index = ~index;

        list.Insert(index, value);
        return index;
    }
}


[Serializable]
public class ScoreHistory : CappedRankedList<int>
{
    // only hold top 10 scores in score history
    public ScoreHistory() : base(10) { }
}

[Serializable]
public class LeaderboardRanking : CappedRankedList<LeaderboardData>
{
    // hold the top 50 scores
    public LeaderboardRanking() : base(50) { }
}
