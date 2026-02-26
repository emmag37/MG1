using UnityEngine;
using System;

public class Board : MonoBehaviour
{
    // Events
    public event Action BoardFull;

    // store the grid children here
    private Cell[,] grid = new Cell[5, 5];

    private int num_filled;
    private int wc = 6;

    void Awake()
    {
        // initialize variables
        num_filled = 0;

        // access the cells from the game scene
        int index = 0;
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
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
        num_filled++;

        // check for a five in a row
        int points = FiveInRow(index, num);
        if (points > 0) num_filled--;   // row cleared, update grid to remove player

        // check for a game over
        if (num_filled == 25) BoardFull?.Invoke();

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
        // O(n)
    public void Reset()
    {
        foreach (Cell cell in grid)
        {
            cell.Reset();
        }

        // reset variables
        num_filled = 0;
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
            if (grid[index.x, i].GetColor() != num && grid[index.x, i].GetColor() != wc) row = false;

            // check the col
            if (grid[i, index.y].GetColor() != num && grid[i, index.y].GetColor() != wc) col = false;

            // check the right diag
            if (r_diag && grid[i, i].GetColor() != num && grid[i, i].GetColor() != wc) r_diag = false;

            // check the left diag
            if (l_diag && grid[4 - i, i].GetColor() != num && grid[4 - i, i].GetColor() != wc) l_diag = false;
        }

        int counts = 0;
        if (row)
        {
            ClearRow(index.x);
            counts++;
        }
        if (col)
        {
            ClearCol(index.y);
            counts++;
        }
        if (r_diag)
        {
            ClearRDiag();
            counts++;
        }
        if (l_diag)
        {
            ClearLDiag();
            counts++;
        }

        return UpdateCounts(counts);
    }

    // updates grid count and returns points scored
    private int UpdateCounts(int count)
    {
        // can add combo scores later
        num_filled -= 4 * count;
        return 5 * count;
    }

    private void ClearRow(int row)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            grid[row, i].Reset();
        }
    }

    private void ClearCol(int col)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            grid[i, col].Reset();
        }
    }

    private void ClearRDiag()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            grid[i, i].Reset();
        }
    }

    private void ClearLDiag()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            grid[4 - i, i].Reset();
        }
    }
}
 