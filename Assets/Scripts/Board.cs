using UnityEngine;
using System;

public class Board : MonoBehaviour
{
    private const int RowSize = 5;

    // Events
    public event Action BoardFull;

    // store the grid children here
    private Cell[,] grid = new Cell[RowSize, RowSize];

    // keep track of filled spots
    private int[] rowCounts = new int[RowSize];
    private int[] colCounts = new int[RowSize];

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
    // returns the points scored
    // updates numFilled
    // works by only scanning when a row/col is filled
    private int FiveInRow(Vector2Int index, int color)
    {
        int pointsScored = 0;
        int row = index.x;
        int col = index.y;

        // add to the board
        rowCounts[row]++;
        colCounts[col]++;

        bool clearRow = rowCounts[row] == RowSize && ScanLineColors(i => (row, i), color);
        bool clearCol = colCounts[col] == RowSize && ScanLineColors(i => (i, col), color);

        // clear filled lines
        if (clearRow)
        {
            ClearLine(i => (row, i));   // clear grid[row, i]
            rowCounts[row] = 0;

            // update the column counts
            for (int i = 0; i < RowSize; i++) colCounts[i] = DecrementCount(colCounts[i]);

            pointsScored += 5;
        }
        if (clearCol)
        {
            ClearLine(i => (i, col));   // clear grid[i, col]
            colCounts[col] = 0;

            // update the row counts
            for (int i = 0; i < RowSize; i++) rowCounts[i] = DecrementCount(rowCounts[i]);

            pointsScored += 5;
        }

        // update the numFilled with the player if no lines cleared
        if (pointsScored == 0) numFilled++;

        return pointsScored;
    }

    // param is a lambda for the index, and returns whether a line is all the same color
    private bool ScanLineColors(Func<int, (int r, int c)> indexSelector, int color)
    {
        for (int i = 0; i < RowSize; i++)
        {
            var (r, c) = indexSelector(i);
            if (grid[r, c].GetColor() != color) return false;
        }

        return true;
    }

    // parameter is a lambda function for line clearing logic
    private void ClearLine(Func<int, (int r, int c)> indexSelector)
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

    /*
    // Checks for a 5 in a row in all directions, clears row if necessary
    // Runs in O(n)
    private int FiveInRow(Vector2Int index, int num)
    {
        // bug: does not clear if the wc is the placed tile that clears the row
        // add a function that picks a color in the row/col/diag, set that as the num to compare to

        bool row = true;
        bool col = true;
        bool r_diag = (index.x == index.y);     // only check if the index is actually on the diagonal
        bool l_diag = (4 - index.x == index.y);

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            // check the row
            if (row) row = CheckSameColor(num, grid[index.x, i].GetColor());

            // check the col
            if (col) col = CheckSameColor(num, grid[i, index.y].GetColor());

            // check the right diag
            if (r_diag) r_diag = CheckSameColor(num, grid[i, i].GetColor());

            // check the left diag
            if (l_diag) l_diag = CheckSameColor(num, grid[grid.GetLength(0) - 1 - i, i].GetColor());
        }

        int pointsScored = ClearFullLines(index, row, col, r_diag, l_diag);

        return pointsScored;
    }

    // returns true if player matches current
    private bool CheckSameColor(int player, int current)
    {
        return (current == player) || (current == wc);
    }

    // returns the points scored
    private int ClearFullLines(Vector2Int index, bool row, bool col, bool r_diag, bool l_diag)
    {
        int count = 0;
        if (row)
        {
            ClearLine(i => (index.x, i));   // clear grid[row, i]
            count++;
        }
        if (col)
        {
            ClearLine(i => (i, index.y));   // clear grid[i, col]
            count++;
        }
        if (r_diag)
        {
            ClearLine(i => (i, i));         // clear grid[i, i]
            count++;
        }
        if (l_diag)
        {
            ClearLine(i => (grid.GetLength(0) - 1 - i, i));     // clear grid[4 - i, i]
            count++;
        }

        // update the number of filled spaces on the grid
        numFilled -= 4 * count;

        // return the points scored - think about adding combo scores later
        return 5 * count;
    } */


}
 