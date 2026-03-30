using UnityEngine;
using System;

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
    // Unity Lifecycle Methods
    // ================================

    void Awake()
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

        EventBus.Publish(new PlacePlayerEvent { PlayerPosition = newPosition });

        RunPlay(index, color);
    }

    // used for the ghost preview
    public void TryGhostPreview(Vector2Int index, CellColor color)
    {
        bool preview = logic.ValidCell(index.x, index.y, color);

        if (preview)
        {
            EventBus.Publish(new GhostPreviewEvent
            {
                Index = index,
                OriginalColor = logic.GetCellColor(index.x, index.y)
            });
        }
    }

    /// <summary>
	/// Resets the board to empty cells.
	/// </summary>
    public void Reset()
    {
        logic.ResetBoard();
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
        EventBus.Publish(new SetCellEvent { Index = index, Color = color });

        BoardLogic.PlayResult result;
        if (!logic.TryPlacePlayer(index.x, index.y, color, out result))     // run the board logic
        {
            Debug.LogError($"Ran play with invalid index or color: {index}, {color}");
        }

        if (result.FullBoard)
        {
            EventBus.Publish(new FullBoardEvent());                    // activate a game over
            return;
        }
            
        // set full rows to empty cells
        if (result.ClearRow) EventBus.Publish(new ClearRowEvent { Row = index.x });
        if (result.ClearCol) EventBus.Publish(new ClearColumnEvent { Column = index.y });
        if (result.ClearRDiag) EventBus.Publish(new ClearRightDiagEvent());
        if (result.ClearLDiag) EventBus.Publish(new ClearLeftDiagEvent());

        if (result.Points > 0) EventBus.Publish(new WinEvent { Points = result.Points });
        EventBus.Publish(new TurnCompletedEvent());
    }

}
