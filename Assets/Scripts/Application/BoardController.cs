using UnityEngine;
using System;
using System.Collections;

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

    //private Coroutine ghostPreview;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        logic = new BoardLogic();
        
    }

    void OnEnable()
    {
        // Player Events
        EventBus.Subscribe<PlayerDraggingEvent>(OnPlayerDragging);
    }

    void OnDisable()
    {
        // Player Events
        EventBus.Unsubscribe<PlayerDraggingEvent>(OnPlayerDragging);
    }

    // ================================
    // Public Methods
    // ================================

    public void PlacePlayer(Vector2Int index, CellColor color)
    {
        if (!logic.ValidCell(index.x, index.y, color))
        {
            EventBus.Publish(new ReturnPlayerEvent());
            return;
        }

        EventBus.Publish(new PlayerOnBoardEvent { Index = index });

        RunPlay(index, color);
    }

    // ================================
    // Player Event Handlers
    // ================================

    private void OnPlayerDragging(PlayerDraggingEvent e)
    {
        //ghostPreview = StartCoroutine(GhostPreviewLoop(e.PlayerTransform, e.Color));
    }

    // ================================
    // Private Methods
    // ================================

    /// <summary>
	/// Resets the board to empty cells.
	/// </summary>
    private void Reset()
    {
        logic.ResetBoard();
    }


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


    // ================================
    // Coroutines
    // ================================
    /*
    IEnumerator GhostPreviewLoop(Transform playerTransform, CellColor color)
    {
        bool ghostSet = false;

        Vector2Int index = Vector2Int.zero;
        CellColor ghostColor = CellColor.Empty;

        while (true)
        {
            Vector3 position = playerTransform.position;

            bool valid = TryGetPlayerPosition(position, color, out Vector3 cellPos, out Vector2Int newIndex);

            // remove ghost preview
            if (ghostSet && (!valid || newIndex != index))
            {
                boardView.SetCell(index.x, index.y, ghostColor);
                ghostSet = false;
            }

            // set new ghost preview
            if (!ghostSet && valid)
            {
                index = newIndex;
                ghostColor = logic.GetCellColor(index.x, index.y);

                boardView.SetCell(index.x, index.y, CellColor.Shadow);
                ghostSet = true;
            }

            yield return null;
        }
    }*/

}
