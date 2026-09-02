using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

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
        NextColor = CellColor.Empty;
    }

    public void InitializeNextColor()
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
        int i = cells.FindIndex(c => c.x == entry.x && c.y == entry.y);
        if (i >= 0)
            cells[i] = entry;
        else
            cells.Add(entry);
    }

    public void Reset() => cells.Clear();
}

public static class BoardDataPrinter
{
    public static void PrintGrid(BoardData data)
    {
        int size = GameConstants.RowSize;

        var lookup = new Dictionary<(int x, int y), CellEntry>();
        foreach (var entry in data.Cells)
            lookup[(entry.x, entry.y)] = entry;

        var sb = new StringBuilder();
        sb.AppendLine($"Board state ({data.Cells.Count} cells):");

        for (int y = size - 1; y >= 0; y--) // top row first
        {
            sb.Append('|');
            for (int x = 0; x < size; x++)
            {
                string cellText = lookup.TryGetValue((x, y), out var entry)
                    ? ((int)entry.color).ToString()
                    : "-"; // empty cell (no CellEntry present)

                sb.Append(cellText.PadLeft(2)).Append('|');
            }
            sb.AppendLine();
        }

        Debug.Log(sb.ToString());
    }
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
