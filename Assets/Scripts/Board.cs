using UnityEngine;
using System;

public class Board : MonoBehaviour
{
    private const int RowSize = 5;
    private const int WildCard = 6;

    // Events
    public event Action BoardFull;

    // store the grid children here
    private Cell[,] grid = new Cell[RowSize, RowSize];

    // keep track of filled spots
    private int[] rowCounts = new int[RowSize];
    private int[] colCounts = new int[RowSize];
    private int rDiagCount;
    private int lDiagCount;

    private int numFilled;

    void Awake()
    {
        // initialize variables
        numFilled = 0;

        // access the cells from the game scene
        int index = 0;
        for (int x = 0; x < RowSize; x++)
        {
            // initialize the cells
            for (int y = 0; y < RowSize; y++)
            {
                grid[x, y] = transform.GetChild(index).GetComponent<Cell>();
                index++;
            }
        }
    }

    public Cell GetCell(int x, int y)
    {
        return grid[x, y];
    }

    // returns the points scored on the turn
    public int SetFilled(Vector2Int pos, Sprite sprite, int num)
    {
        // adjust the world pos to match grid indices
        Vector2Int index = WorldPosToIndex(pos);

        // set the cell at pos
        grid[index.x, index.y].AssignSprite(sprite, num);

        // add the player to the board - clears full rows and returns any points scored
        int points = FiveInRow(index, num);

        // check for a game over
        if (numFilled == RowSize * RowSize) BoardFull?.Invoke();

        return points;
    }

    // returns whether or not the spot is already filled
    public bool IsFilled(Vector2Int pos)
    {
        Vector2Int index = WorldPosToIndex(pos);

        // empty cells will contain the value -1
        return grid[index.x, index.y].GetColor() != -1;
    }

    // resets the board to empty slots
    public void Reset()
    {
        foreach (Cell cell in grid)
        {
            cell.Reset();
        }

        // reset fill state variables
        numFilled = 0;
        rDiagCount = 0;
        lDiagCount = 0;
        for (int i = 0; i < RowSize; i++)
        {
            rowCounts[i] = 0;
            colCounts[i] = 0;
        }
    }

    // converts the world row, col to the grid index
    private Vector2Int WorldPosToIndex(Vector2Int pos)
    {
        // where do these Vector2s exist?
        Vector2Int index = new Vector2Int();

        index.x = (pos.x * -1) + 2;   // reverse row direction first
        index.y = pos.y + 2;

        return index;
    }

    // still working on this
    // returns the points scored, updates numFilled, works by only scanning when a row/col is filled (worst case O(n) but generally O(1))
    private int FiveInRow(Vector2Int index, int color)
    {
        int row = index.x;
        int col = index.y;

        // add to the board
        rowCounts[row]++;
        colCounts[col]++;
        if (row == col) rDiagCount++;
        if (RowSize - 1 - row == col) lDiagCount++;

        bool clearRow = (rowCounts[row] == RowSize) && ScanLineColors(i => (row, i), color);
        bool clearCol = (colCounts[col] == RowSize) && ScanLineColors(i => (i, col), color);
        bool clearRDiag = (rDiagCount == RowSize) && ScanLineColors(i => (i, i), color);
        bool clearLDiag = (lDiagCount == RowSize) && ScanLineColors(i => (RowSize - 1 - i, i), color);

        // clear filled lines
        if (clearRow) ClearRow(row);
        if (clearCol) ClearColumn(col);
        if (clearRDiag) ClearRDiagonal();
        if (clearLDiag) ClearLDiagonal();

        // calculate points
        int pointsScored = CalculatePoints(clearRow, clearCol, clearRDiag, clearLDiag);

        // update the numFilled with the player if no lines cleared
        if (pointsScored == 0) numFilled++;

        return pointsScored;
    }

    // returns the number of points scored
    private int CalculatePoints(bool row, bool col, bool rDiag, bool lDiag)
    {
        int linesCleared = (row ? 1 : 0) + (col ? 1 : 0) + (rDiag ? 1 : 0) + (lDiag ? 1 : 0);

        return linesCleared * linesCleared * 5; // multiply by lines cleared again for the combo score
    }

    // param is a lambda for the index, and returns whether a line is all the same color
        // add the wild card logic to this function
    private bool ScanLineColors(Func<int, (int r, int c)> indexSelector, int color)
    {
        for (int i = 0; i < RowSize; i++)
        {
            var (r, c) = indexSelector(i);
            int cellColor = grid[r, c].GetColor();

            // need to set the color if a wild card
            if (color == WildCard) color = cellColor;

            // check for color matches
            if (cellColor != color && cellColor != WildCard) return false;
        }

        return true;
    }

    // parameter is a lambda function for line clearing logic
    private void ClearGridLine(Func<int, (int r, int c)> indexSelector)
    {
        for (int i = 0; i < RowSize; i++)
        {
            var (r, c) = indexSelector(i);
            grid[r, c].Reset();
        }

        numFilled -= 4;
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
        ClearGridLine(i => (row, i));   // clear grid[row, i]
        rowCounts[row] = 0;

        // update the other counts
        for (int i = 0; i < RowSize; i++) colCounts[i] = DecrementCount(colCounts[i]);
        rDiagCount = DecrementCount(rDiagCount);
        lDiagCount = DecrementCount(lDiagCount);
    }
    private void ClearColumn(int col)
    {
        ClearGridLine(i => (i, col));   // clear grid[i, col]
        colCounts[col] = 0;

        // update the other counts
        for (int i = 0; i < RowSize; i++) rowCounts[i] = DecrementCount(rowCounts[i]);
        rDiagCount = DecrementCount(rDiagCount);
        lDiagCount = DecrementCount(lDiagCount);
    }
    private void ClearRDiagonal()
    {
        ClearGridLine(i => (i, i));      // clear grid[i, i]
        rDiagCount = 0;

        // update the other counts
        for (int i = 0; i < RowSize; i++) colCounts[i] = DecrementCount(colCounts[i]);
        for (int i = 0; i < RowSize; i++) rowCounts[i] = DecrementCount(rowCounts[i]);
        lDiagCount = DecrementCount(lDiagCount);
    }
    private void ClearLDiagonal()
    {
        ClearGridLine(i => (RowSize - 1 - i, i));
        lDiagCount = 0;

        // update the other counts
        for (int i = 0; i < RowSize; i++) colCounts[i] = DecrementCount(colCounts[i]);
        for (int i = 0; i < RowSize; i++) rowCounts[i] = DecrementCount(rowCounts[i]);
        rDiagCount = DecrementCount(rDiagCount);
    }

}
