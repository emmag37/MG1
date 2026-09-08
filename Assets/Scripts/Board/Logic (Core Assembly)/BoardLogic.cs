using System;
using System.Collections.Generic;


/// <summary>
/// Manages a representation of the board.
/// Respsonsible for row checking/clearing logic and point calculation.
/// </summary>
public class BoardLogic
{
    // ==================================================
    // Constants
    // ==================================================
    private const int NumSpots = RowSize * RowSize;
    private const int RowSize = GameConstants.RowSize;

    private const CellColor Empty = CellColor.Empty;
    private const CellColor Mask = CellColor.Mask;
    private const CellColor WildCard = CellColor.WildCard;

    // ==================================================
    // Public Types
    // ==================================================
    public struct PlayResult
    {
        public int Points;

        public bool ClearRow;
        public bool ClearCol;
        public bool ClearRDiag;
        public bool ClearLDiag;

        public bool FullBoard;

    }

    // ==================================================
    // Private Fields
    // ==================================================
    private int[] rowCounts = new int[RowSize];
    private int[] colCounts = new int[RowSize];
    private int rDiagCount = 0;
    private int lDiagCount = 0;

    private int numSpotsFilled = 0;
    private CellColor[,] gridColors = new CellColor[RowSize, RowSize];

    private (int, int)[] liveZone;

    // ==================================================
    // Public Methods
    // ==================================================

    public int AddCellsToBoard(IReadOnlyList<CellEntry> cells)
    {
        int numAdded = 0;

        if (cells == null)
        {
            Logger.Error("[BoardLogic] Cells passed to AddCellsToBoard are null");
            return numAdded;
        }

        foreach (CellEntry cell in cells)
        {
            if (AddToBoard(cell.x, cell.y, cell.color))
                numAdded++;
        }

        return numAdded;
    }

    public BoardData FillBoardData()
    {
        BoardData data = new BoardData();

        for (int row = 0; row < RowSize; row++)
        {
            for (int col = 0; col < RowSize; col++)
            {
                if (gridColors[row, col] != Empty)
                {
                    CellEntry entry = new CellEntry(row, col, gridColors[row, col]);
                    data.Set(entry);                                
                }
            }
        }

        return data;
    }

    public CellColor GetCellColor(int row, int col)
    {
        if (row < 0 || row >= RowSize || col < 0 || col >= RowSize)
            throw new IndexOutOfRangeException($"({row}, {col})");

        return gridColors[row, col];
    }

    public bool ValidCell(int row, int col, CellColor color)
    {
        bool valid =
            (row >= 0 && row < RowSize) &&
            (col >= 0 && col < RowSize) &&
            (color == Mask || gridColors[row, col] == Empty) &&
            IsLivePos((row, col));

        return valid;
    }

    public void ResetBoard()
    {
        for (int i = 0; i < RowSize; i++)
        {
            rowCounts[i] = 0;
            colCounts[i] = 0;
        }
        rDiagCount = 0;
        lDiagCount = 0;

        // reset the grid
        numSpotsFilled = 0;
        ResetColors();

        AddLiveZone(null);
    }

    public bool TryPlacePlayer(int row, int col, CellColor color, out PlayResult result)
    {
        result = new PlayResult();
        if (!ValidCell(row, col, color) || !AddToBoard(row, col, color))
            return false;

        result = CalculateLines(row, col, color);

        return true;
    }

    // indices must be checked on access
    public void AddLiveZone((int, int)[] indices)
    {
        liveZone = indices;
    }


    // ==================================================
    // Internal Methods - Testing Only
    // ==================================================

    internal int GetSpotsFilled() => numSpotsFilled;

    // ==================================================
    // Private Methods
    // ==================================================

    // places the player on the board and sets play result values
    // assumes row, col, and color are correct
    private PlayResult CalculateLines(int row, int col, CellColor color)
    {
        PlayResult result = new PlayResult();

        // check for full and matching lines
        result.ClearRow =
            (rowCounts[row] == RowSize) &&
            LineColorsMatch(i => (row, i), color);

        result.ClearCol =
            (colCounts[col] == RowSize) &&
            LineColorsMatch(i => (i, col), color);

        result.ClearRDiag =
            (rDiagCount == RowSize) &&
            LineColorsMatch(i => (i, i), color);

        result.ClearLDiag =
            (lDiagCount == RowSize) &&
            LineColorsMatch(i => (RowSize - 1 - i, i), color);

        // clear any filled lines
        if (result.ClearRow) ClearRow(row);
        if (result.ClearCol) ClearColumn(col);
        if (result.ClearRDiag) ClearRDiagonal();
        if (result.ClearLDiag) ClearLDiagonal();

        if (result.ClearRow || result.ClearCol || result.ClearRDiag || result.ClearLDiag) // any line cleared
            numSpotsFilled--;

        result.Points = CalculatePoints(result);
        result.FullBoard = (numSpotsFilled == NumSpots);

        return result;
    }

    private bool AddToBoard(int row, int col, CellColor color)
    {
        // check out of bounds/bad color
        if (color == Empty || !ValidCell(row, col, color))
        {
            Logger.Error($"[BoardLogic] Invalid args passed to AddToBoard: ({row}, {col}, {color})");
            return false;
        }

        // increase the counts if not mask
        if (gridColors[row, col] == Empty)
        {
            rowCounts[row]++;
            colCounts[col]++;
            if (row == col) rDiagCount++;
            if (RowSize - 1 - row == col) lDiagCount++;

            numSpotsFilled++;
        }

        // set the color
        gridColors[row, col] = color;
        return true;
    }

    // sets grid colors to empty
    private void ResetColors()
    {
        for (int x = 0; x < RowSize; x++)
        {
            for (int y = 0; y < RowSize; y++)
            {
                gridColors[x, y] = Empty;
            }
        }
    }

    // for the live zone - tutorial use only
    private bool IsLivePos((int, int) index)
    {
        if (index.Item1 < 0 || index.Item1 >= RowSize || index.Item2 < 0 || index.Item2 >= RowSize)
            throw new IndexOutOfRangeException($"{index}");

        if (liveZone == null)
            return true;

        foreach ((int, int) pos in liveZone)
        {
            if (index == pos)
                return true;
        }

        return false;
    }

    // ==================================================
    // Play Result Helper Methods
    // ==================================================

    // sets the number of points scored in result
    private int CalculatePoints(PlayResult result)
    {
        int linesCleared =
            (result.ClearRow ? 1 : 0) +
            (result.ClearCol ? 1 : 0) +
            (result.ClearRDiag ? 1 : 0) +
            (result.ClearLDiag ? 1 : 0);

        int points = linesCleared * linesCleared * 5; // multiply by lines cleared again for the combo score
        return points;
    }

    // returns whether the line indicated by indexSelector is all color, assumes that the line is full
    private bool LineColorsMatch(Func<int, (int r, int c)> indexSelector, CellColor color)
    {
        if (color == Mask) color = WildCard;    // cast to wild card

        for (int i = 0; i < RowSize; i++)
        {
            var (r, c) = indexSelector(i);
            CellColor cellColor = gridColors[r, c];

            if (cellColor == Mask) cellColor = WildCard;    // cast to wc if mask

            if (color == WildCard) color = cellColor;   // pick color to compare to if color is WildCard

            if (cellColor != color && cellColor != WildCard) return false;
        }

        return true;
    }

    // safe count decrementer, ensures count never goes below 0
    private int DecrementCount(int count)
    {
        if (count > 0) return count - 1;
        return 0;
    }

    // Line Clearing Helpers
    private void ClearRow(int row)
    {
        SetLineEmpty(i => (row, i));
        rowCounts[row] = 0;
        
        for (int i = 0; i < RowSize; i++) colCounts[i] = DecrementCount(colCounts[i]);
        rDiagCount = DecrementCount(rDiagCount);
        lDiagCount = DecrementCount(lDiagCount);
    }
    private void ClearColumn(int col)
    {
        SetLineEmpty(i => (i, col));
        colCounts[col] = 0;

        for (int i = 0; i < RowSize; i++) rowCounts[i] = DecrementCount(rowCounts[i]);
        rDiagCount = DecrementCount(rDiagCount);
        lDiagCount = DecrementCount(lDiagCount);
    }
    private void ClearRDiagonal()
    {
        SetLineEmpty(i => (i, i));
        rDiagCount = 0;

        for (int i = 0; i < RowSize; i++) colCounts[i] = DecrementCount(colCounts[i]);
        for (int i = 0; i < RowSize; i++) rowCounts[i] = DecrementCount(rowCounts[i]);
        lDiagCount = DecrementCount(lDiagCount);
    }
    private void ClearLDiagonal()
    {
        SetLineEmpty(i => (RowSize - 1 - i, i));
        lDiagCount = 0;

        for (int i = 0; i < RowSize; i++) colCounts[i] = DecrementCount(colCounts[i]);
        for (int i = 0; i < RowSize; i++) rowCounts[i] = DecrementCount(rowCounts[i]);
        rDiagCount = DecrementCount(rDiagCount);
    }

    private void SetLineEmpty(Func<int, (int r, int c)> indexSelector)
    {
        for (int i = 0; i < RowSize; i++)
        {
            var (r, c) = indexSelector(i);
            gridColors[r, c] = Empty;
        }

        numSpotsFilled -= 4;
    }
}
