using UnityEngine;
using System;

public class Board : MonoBehaviour
{
    // ================================
    // Public Types
    // ================================

    // ================================
    // Constants
    // ================================

    // ================================
    // Events
    // ================================

    // ================================
    // Inspector Fields
    // ================================

    // ================================
    // Private Fields
    // ================================

    // ================================
    // Unity Lifecycle Methods
    // ================================

    // ================================
    // Public Methods
    // ================================

    // ================================
    // Private Methods
    // ================================

    private BoardLogic logic;

    private const int RowSize = 5;

    // Events
    public event Action BoardFull;

    // store the grid children here
    private Cell[,] grid = new Cell[RowSize, RowSize];

    void Awake()
    {
        logic = new BoardLogic();

        // access the cells from the game scene
        int index = 0;
        for (int x = 0; x < RowSize; x++)
        {
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
    public int AddToBoard(Vector2Int index, Sprite sprite, int num)
    {
        grid[index.x, index.y].AssignSprite(sprite, num);           // render the player

        var result = logic.PlacePlayer(index, num);                 // run the play calculations
        if (result.FullBoard) BoardFull?.Invoke();                  // activate a game over

        // render empty sprites for full lines
        if (result.ClearRow) ClearGridLine(i => (index.x, i));
        if (result.ClearCol) ClearGridLine(i => (i, index.y));
        if (result.ClearRDiag) ClearGridLine(i => (i, i));
        if (result.ClearLDiag) ClearGridLine(i => (RowSize - 1 - i, i));

        return result.Points;
    }

    // returns whether or not the spot is already filled
    public bool IsFilled(Vector2Int index)
    {
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
