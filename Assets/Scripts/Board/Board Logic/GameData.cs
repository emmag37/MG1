using UnityEngine;
using System;
using System.Collections.Generic;

// rename to board data? or the new game data

[Serializable]
public struct CellEntry
{
    public int x, y, color;
    public CellEntry(int x, int y, int color) { this.x = x; this.y = y; this.color = color; }
}

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
