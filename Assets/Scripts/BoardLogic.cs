using UnityEngine;
using System;

public class BoardLogic
{
    // ================================
    // Public Types
    // ================================
    public struct PlayResult
    {
        public int Points;

        public bool ClearRow;
        public bool ClearCol;
        public bool ClearRDiag;
        public bool ClearLDiag;

        public bool FullBoard;

    }


    // ================================
    // Constants
    // ================================
    private const int RowSize = 5;
    private const int WildCard = 6;
    private const int Empty = -1;


    // ================================
    // Private Fields
    // ================================
    private int[] rowCounts = new int[RowSize];
    private int[] colCounts = new int[RowSize];
    private int rDiagCount = 0;
    private int lDiagCount = 0;

    private int numSpotsFilled = 0;
    private int[,] gridColors = new int[RowSize, RowSize];


    // ================================
    // Public Methods
    // ================================

    // need a constructor to initialize variables - modify this description
    public BoardLogic()
    {
        // Set the colors on the grid to empty
        ResetGrid();
    }

    // Add a longer description of this method
    public bool PosIsFilled(int row, int col)
    {
        return gridColors[row, col] != Empty;
    }

    // Add a longer description of this method
    public void ResetBoard()
    {
        // Reset counts
        for (int i = 0; i < RowSize; i++)
        {
            rowCounts[i] = 0;
            colCounts[i] = 0;
        }
        rDiagCount = 0;
        lDiagCount = 0;

        // Reset the grid
        numSpotsFilled = 0;
        ResetGrid();
    }

    // Add a longer description of this method
    public PlayResult PlacePlayer(Vector2Int index, int color)
    {
        PlayResult result = new PlayResult();

        int row = index.x;
        int col = index.y;

        // Add player to the board
        gridColors[row, col] = color;

        rowCounts[row]++;
        colCounts[col]++;
        if (row == col) rDiagCount++;
        if (RowSize - 1 - row == col) lDiagCount++;

        // Check for full and matching lines
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

        // Clear any filled lines
        if (result.ClearRow) ClearRow(row);
        if (result.ClearCol) ClearColumn(col);
        if (result.ClearRDiag) ClearRDiagonal();
        if (result.ClearLDiag) ClearLDiagonal();

        CalculatePoints(result);

        if (result.Points == 0) numSpotsFilled++;   // The player fills a spot if no lines cleared

        result.FullBoard = (numSpotsFilled == RowSize * RowSize);   // checks if there is a full board

        return result;
    }


    // ================================
    // Private Methods
    // ================================

    // sets grid colors to empty
    private void ResetGrid()
    {
        for (int x = 0; x < RowSize; x++)
        {
            for (int y = 0; y < RowSize; y++)
            {
                gridColors[x, y] = Empty;
            }
        }
    }

    // sets the number of points scored in result
    private void CalculatePoints(PlayResult result)
    {
        int linesCleared =
            (result.ClearRow ? 1 : 0) +
            (result.ClearCol ? 1 : 0) +
            (result.ClearRDiag ? 1 : 0) +
            (result.ClearLDiag ? 1 : 0);

        result.Points = linesCleared * linesCleared * 5; // multiply by lines cleared again for the combo score
    }

    // returns whether the line indicated by indexSelector is all color, assumes that the line is full
    private bool LineColorsMatch(Func<int, (int r, int c)> indexSelector, int color)
    {
        for (int i = 0; i < RowSize; i++)
        {
            var (r, c) = indexSelector(i);
            int cellColor = gridColors[r, c];

            if (color == WildCard) color = cellColor;   // pick color to compare to if color is WildCard

            if (cellColor != color && cellColor != WildCard) return false;
        }

        return true;
    }

    // safe count decrementer, ensures count never goes below 0
    private int DecrementCount(int count)
    {
        if (count > 0)
        {
            return count - 1;
        }
        return 0;
    }

    // Line Clearing Helpers
    private void ClearRow(int row)
    {
        rowCounts[row] = 0;

        // update the other counts
        for (int i = 0; i < RowSize; i++) colCounts[i] = DecrementCount(colCounts[i]);
        rDiagCount = DecrementCount(rDiagCount);
        lDiagCount = DecrementCount(lDiagCount);
    }
    private void ClearColumn(int col)
    {
        colCounts[col] = 0;

        // update the other counts
        for (int i = 0; i < RowSize; i++) rowCounts[i] = DecrementCount(rowCounts[i]);
        rDiagCount = DecrementCount(rDiagCount);
        lDiagCount = DecrementCount(lDiagCount);
    }
    private void ClearRDiagonal()
    {
        rDiagCount = 0;

        // update the other counts
        for (int i = 0; i < RowSize; i++) colCounts[i] = DecrementCount(colCounts[i]);
        for (int i = 0; i < RowSize; i++) rowCounts[i] = DecrementCount(rowCounts[i]);
        lDiagCount = DecrementCount(lDiagCount);
    }
    private void ClearLDiagonal()
    {
        lDiagCount = 0;

        // update the other counts
        for (int i = 0; i < RowSize; i++) colCounts[i] = DecrementCount(colCounts[i]);
        for (int i = 0; i < RowSize; i++) rowCounts[i] = DecrementCount(rowCounts[i]);
        rDiagCount = DecrementCount(rDiagCount);
    }
}
