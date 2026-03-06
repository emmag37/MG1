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
    private BoardGeometry boardGeometry;

    private GamePieceImage[,] grid = new GamePieceImage[RowSize, RowSize];  // grid children


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        logic = new BoardLogic();

        boardGeometry = GetComponent<BoardGeometry>();

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
    public int AddToBoard(Vector3 position, int color)
    {
        Vector2Int index = new Vector2Int(0, 0);        // need to convert the position to the grid index

        grid[index.x, index.y].SetSprite(color);                      // render the player on the board

        var result = logic.PlacePlayer(index, color);                 // run the play calculations
        if (result.FullBoard) BoardFull?.Invoke();                  // activate a game over

        // render empty sprites for full lines
        if (result.ClearRow) ClearGridLine(i => (index.x, i));
        if (result.ClearCol) ClearGridLine(i => (i, index.y));
        if (result.ClearRDiag) ClearGridLine(i => (i, i));
        if (result.ClearLDiag) ClearGridLine(i => (RowSize - 1 - i, i));

        return result.Points;
    }

    // calculates the new postion of the player on the board. returns positive infinity if invalid
    // add a longer description
    public Vector3 GetNewPlayerPosition(Vector3 position)
    {
        return Vector3.positiveInfinity;
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
