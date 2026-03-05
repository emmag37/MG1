/**
 * Insert File Description
 * 
 */

using UnityEngine;
using System;

public class Board : MonoBehaviour
{
    // ================================
    // Constants
    // ================================
    private const int RowSize = 5;

    // ================================
    // Events
    // ================================
    public event Action BoardFull;

    // ================================
    // Private Fields
    // ================================
    private BoardLogic logic;
    private GamePieceImage[,] grid = new GamePieceImage[RowSize, RowSize];  // grid children


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        logic = new BoardLogic();

        // access the cells from the game scene
        int index = 0;
        for (int x = 0; x < RowSize; x++)
        {
            for (int y = 0; y < RowSize; y++)
            {
                grid[x, y] = transform.GetChild(index).GetComponent<GamePieceImage>();
                index++;
            }
        }
    }


    // ================================
    // Public Methods
    // ================================

    // returns the points scored on the turn
    // add a longer description
    public int AddToBoard(Vector2Int index, int color)
    {
        grid[index.x, index.y].SetSprite(color);                      // render the player on the board

        Debug.Log("add color " + color + " to the board");

        var result = logic.PlacePlayer(index, color);                 // run the play calculations
        if (result.FullBoard) BoardFull?.Invoke();                  // activate a game over

        // render empty sprites for full lines
        if (result.ClearRow) ClearGridLine(i => (index.x, i));
        if (result.ClearCol) ClearGridLine(i => (i, index.y));
        if (result.ClearRDiag) ClearGridLine(i => (i, i));
        if (result.ClearLDiag) ClearGridLine(i => (RowSize - 1 - i, i));

        return result.Points;
    }

    // returns whether or not the spot is already filled
    // add a longer description
    public bool IsFilled(Vector2Int index)
    {
        return logic.PosIsFilled(index.x, index.y);
    }

    // resets the board to empty slots
    // add a longer description
    public void Reset()
    {
        foreach (GamePieceImage image in grid)
        {
            image.ResetPiece();
        }

        logic.ResetBoard();
    }


    // ================================
    // Private Methods
    // ================================

    private void ClearGridLine(Func<int, (int r, int c)> indexSelector)
    {
        for (int i = 0; i < RowSize; i++)
        {
            var (r, c) = indexSelector(i);
            grid[r, c].ResetPiece();
        }
    }
}
