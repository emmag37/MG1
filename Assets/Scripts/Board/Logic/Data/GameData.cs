using UnityEngine;
using System;
using System.Collections.Generic;


[Serializable]
public class GameData
{
    public int Score;
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
        NextColor = CellColor.Empty;
    }

    public void InitializeNextColor()
    {
        PlayerColor = CellColor.Empty;
        NextColor = PlayerPicker.ChooseColor();
    }
}


[Serializable]
public class BoardData
{
    [SerializeField] private List<CellEntry> cells = new();     // sparse list to store board data
    public IReadOnlyList<CellEntry> Cells => cells;

    public void Set(CellEntry entry)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            CellEntry c = cells[i];
            if (c.x == entry.x && c.y == entry.y)
            {
                cells[i] = entry;
                return;
            }
        }

        cells.Add(entry);
    }

    public void Reset() => cells.Clear();
}


[Serializable]
public struct CellEntry
{
    public int x, y;
    public CellColor color;

    public CellEntry(int x, int y, CellColor color)
    {
        if (x < 0 || x >= GameConstants.RowSize || y < 0 || y >= GameConstants.RowSize)
            throw new ArgumentOutOfRangeException(nameof(x), $"CellEntry index out of bounds: ({x}, {y})");
        if (!Enum.IsDefined(typeof(CellColor), color) || color == CellColor.Empty)
            throw new ArgumentOutOfRangeException(nameof(color), $"CellEntry constructed with invalid color: {color}");

        this.x = x;
        this.y = y;
        this.color = color;
    }
}

