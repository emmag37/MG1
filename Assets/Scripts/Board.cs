/**
 * Insert File Description
 * 
 */

using UnityEngine;
using System;

public class Board : MonoBehaviour
{
    private const int RowSize = 5;

    // Events
    public event Action BoardFull;

    // store the grid children here
    private Cell[,] grid = new Cell[RowSize, RowSize];

    private BoardLogic logic;

    void Awake()
    {
        logic = new BoardLogic();

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

    // returns whether or not the spot is already filled
    public bool IsFilled(Vector2Int pos)
    {
        Vector2Int index = WorldPosToIndex(pos);

        return logic.PosIsFilled(index.x, index.y);
    }

    // resets the board to empty slots
    public void Reset()
    {
        foreach (Cell cell in grid)
        {
            cell.Reset();
        }

        logic.ResetBoard();
    }

    // returns the points scored on the turn
    public int AddToBoard(Vector2Int pos, Sprite sprite, int num)
    {
        Vector2Int index = WorldPosToIndex(pos);
        int row = index.x;
        int col = index.y;

        grid[row, col].AssignSprite(sprite, num);           // render the player

        var result = logic.PlacePlayer(index, num);         // runs the play
        if (result.FullBoard) BoardFull?.Invoke();          // initiate a game over, ends function

        // clear filled lines to empty sprites
        if (result.ClearRow) ClearGridLine(i => (row, i));
        if (result.ClearCol) ClearGridLine(i => (i, col));
        if (result.ClearRDiag) ClearGridLine(i => (i, i));
        if (result.ClearLDiag) ClearGridLine(i => (RowSize - 1 - i, i));

        return result.Points;
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

    // parameter is a lambda function for line clearing logic
    private void ClearGridLine(Func<int, (int r, int c)> indexSelector)
    {
        for (int i = 0; i < RowSize; i++)
        {
            var (r, c) = indexSelector(i);
            grid[r, c].Reset();
        }
    }
}
