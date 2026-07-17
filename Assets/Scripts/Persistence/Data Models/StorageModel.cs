using UnityEngine;
using System;
using System.Collections.Generic;

// need to define <, > , == for my leaderboard item

// put this class into its own file
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


// unsure where to put this, most likely UI data
[Serializable]
public class LeaderboardRanking : CappedRankedList<LeaderboardData>
{
    // hold the top 50 scores
    public LeaderboardRanking() : base(50) { }
}


// move this to be with cell

// sparse list to store board data, only ever read from for a list traversal
[Serializable]
public class BoardData
{
    [SerializeField] private List<CellEntry> cells = new();
    public IReadOnlyList<CellEntry> Cells => cells;

    public void Set(int x, int y, int color)
    {
        int i = cells.FindIndex(c => c.x == x && c.y == y);
        if (i >= 0) cells[i] = new CellEntry(x, y, color);
        else cells.Add(new CellEntry(x, y, color));
    }

    public void Reset() => cells.Clear();
}

