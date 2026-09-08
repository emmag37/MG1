using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;


#if UNITY_EDITOR
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

        for (int x = size - 1; x >= 0; x--) // x = row, top row first
        {
            sb.Append('|');
            for (int y = 0; y < size; y++) // y = column
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
#endif
