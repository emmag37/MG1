using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public int Score = 0;   // should always default to 0
    public PlayerColors PlayerColors = new();
    public BoardData Board = new();

    public void Reset()
    {
        Score = 0;
        PlayerColors.Reset();
        Board.Reset();
    }
}

[Serializable]
public class PlayerColors
{
    public CellColor PlayerColor;
    public CellColor NextColor;

    public void Reset()
    {
        PlayerColor = CellColor.Empty;
        NextColor = PlayerPicker.ChooseColor();
    }
}

// sparse list to store board data, only ever read from for a list traversal
[Serializable]
public class BoardData
{
    [SerializeField] private List<CellEntry> cells = new();
    public IReadOnlyList<CellEntry> Cells => cells;

    public void Set(CellEntry entry)
    {
        if (entry == null)

        int i = cells.FindIndex(c => c.x == entry.x && c.y == entry.y);
        if (i >= 0)
            cells[i] = entry;
        else
            cells.Add(entry);
    }

    public void Reset() => cells.Clear();
}

[Serializable]
public struct CellEntry
{
    public int x, y;
    public CellColor color;

    public CellEntry(int x, int y, CellColor color) { this.x = x; this.y = y; this.color = color; }
}
