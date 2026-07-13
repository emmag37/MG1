using UnityEngine;
using System;


// going to turn this into a turn operator

// edit: need to put back resetting the board logic

/// <summary>
/// Manages the gameplay board and its visual representation in the scene.
/// Bridges board logic and geometry with the rendered grid, handles player
/// placement, and clears completed lines.
/// </summary>
public class BoardController: MonoBehaviour
{
    // ================================
    // Constants
    // ================================
    private const int RowSize = GameConstants.RowSize;

    // ================================
    // Private Fields
    // ================================

    private BoardLogic logic;

    // ================================
    // Local Events
    // ================================

    public event Action<int, (int, int)> TurnCompleted;
    public event Action FullBoard;

    // ================================
    // Initializer
    // ================================

    public void Initialize()
    {
        logic = new BoardLogic();
    }


    // ================================
    // Public Methods
    // ================================

    public void TryPlacePlayer(Vector2Int index, CellColor color, Vector3 newPosition)
    {
        if (!logic.ValidCell(index.x, index.y, color))
        {
            EventBus.Publish(new ReturnPlayerEvent());
            return;
        }

        EventBus.Publish(new PlacePlayerEvent
        {
            PlayerPosition = newPosition,
            Index = index,
            Color = color
        });

        RunPlay(index, color);
    }

    // used for the ghost preview
    public bool IsIndexOpen(Vector2Int index, CellColor color)
    {
        return logic.ValidCell(index.x, index.y, color);
    }
    
    /// <summary>
	/// Resets the board to empty cells.
	/// </summary>
    public void Reset()
    {
        logic.ResetBoard();
        EventBus.Publish(new ResetEvent()); // alerts the grid to clear pieces
    }

    // tutorial method
    // deprecate
    public void ClearPieces()
    {
        Reset();

        EventBus.Publish(new ResetEvent()); // alerts the grid to clear pieces
    }

    // used by the tutorial to make sure specific positions are live for players
    public void SetLiveZone((int, int)[] indices)
    {
        logic.AddLiveZone(indices);
    }

    // used by the tutorial and for loading in previous game state
    // note - assumes that there are no players in existence
    // this function is broken
    public void AddNonPlayer(Vector2Int index, CellColor color)
    {
        // add image to grid - no players to listen to this
        Debug.Log("add non player");
        EventBus.Publish(new PlacePlayerEvent
        {
            PlayerPosition = Vector3.positiveInfinity,
            Index = index,
            Color = color
        });

        // fill the spot on the board in logic
        if (!logic.TryPlacePlayer(index.x, index.y, color, out BoardLogic.PlayResult result))     // run the board logic
        {
            Debug.LogError($"Ran play with invalid index or color: {index}, {color}");
        }
    }


    // ================================
    // Private Methods
    // ================================

    /// <summary>
    /// Sets cell to the player's image and clears completed lines.
    /// </summary>
    /// <param name="position">World position of the player to be added.</param>
    /// <param name="color">Color of the player to be added.</param>
    /// <returns>Points scored on the play.</returns>
    /// <remarks>
    /// Invokes <see cref="FullBoard"/> if the board becomes full.
    /// </remarks>
    private void RunPlay(Vector2Int index, CellColor color)
    {
        BoardLogic.PlayResult result;
        if (!logic.TryPlacePlayer(index.x, index.y, color, out result))     // run the board logic
        {
            Debug.LogError($"Ran play with invalid index or color: {index}, {color}");
        }

        if (result.FullBoard)
        {
            FullBoard?.Invoke();                    // activate a game over
            return;
        }

        if (result.Points > 0)
        {
            EventBus.Publish(new WinEvent
            {
                Index = index,

                Row = result.ClearRow,
                Column = result.ClearCol,
                RightDiag = result.ClearRDiag,
                LeftDiag = result.ClearLDiag,

                Points = result.Points
            });
        }

        TurnCompleted?.Invoke(result.Points, (index.x, index.y));
    }

}
