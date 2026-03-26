using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Manages the gameplay board and its visual representation in the scene.
/// Bridges board logic and geometry with the rendered grid, handles player
/// placement, and clears completed lines.
/// </summary>
public class BoardManager: MonoBehaviour
{
    // ================================
    // Constants
    // ================================

    private const int RowSize = GameConstants.RowSize;

    // ================================
    // Inspector Fields
    // ================================

    [SerializeField] private SpriteRenderer boardView;
    [SerializeField] private GridController cellGrid;

    // ================================
    // Private Fields
    // ================================
    private bool initialized = false;

    private BoardLogic logic;
    private BoardGeometry geometry;

    private Coroutine hoverRoutine;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(boardView != null, "Board view not set in board");
        Debug.Assert(cellGrid != null, "Cell grid not set in board");
    }


    void Awake()
    {
        logic = new BoardLogic();
        geometry = new BoardGeometry();
    }

    void OnEnable()
    {
        // Game State Events
        EventBus.Subscribe<StartGameEvent>(OnStartGame);
        EventBus.Subscribe<ExitGameEvent>(OnExitGame);

        // Player Events
        EventBus.Subscribe<PlayerReleasedEvent>(OnPlayerReleased);
        EventBus.Subscribe<PlayerDraggingEvent>(OnPlayerDragging);
    }

    void OnDisable()
    {
        // Game State Events
        EventBus.Unsubscribe<StartGameEvent>(OnStartGame);
        EventBus.Unsubscribe<ExitGameEvent>(OnExitGame);

        // Player Events
        EventBus.Unsubscribe<PlayerReleasedEvent>(OnPlayerReleased);
        EventBus.Unsubscribe<PlayerDraggingEvent>(OnPlayerDragging);
    }


    // ================================
    // Game State Event Handlers
    // ================================

    private void OnStartGame(StartGameEvent e)
    {
        // set active
        boardView.gameObject.SetActive(true);
        cellGrid.gameObject.SetActive(true);

        Initialize();

        EventBus.Publish(new GameReadyEvent { BoardBounds = boardView.bounds }) ;
    }

    private void OnExitGame(ExitGameEvent e)
    {
        Reset();

        // set inactive
        boardView.gameObject.SetActive(false);
        cellGrid.gameObject.SetActive(false);
    }


    // ================================
    // Player Event Handlers
    // ================================

    private void OnPlayerReleased(PlayerReleasedEvent e)
    {
        StopCoroutine(hoverRoutine);

        bool valid = TryGetPlayerPosition(e.PlayerPosition, e.Color, out Vector3 newPos, out Vector2Int index);

        if (valid)
        {
            EventBus.Publish(new PlacePlayerEvent { PlayerPosition = newPos });
            RunPlay(index, e.Color);
        }
        else
        {
            EventBus.Publish(new ReturnPlayerEvent());
        }
    }

    private void OnPlayerDragging(PlayerDraggingEvent e)
    {
        hoverRoutine = StartCoroutine(ShadowHover(e.PlayerTransform));
    }

    // ================================
    // Private Methods
    // ================================

    private void Initialize()
    {
        if (initialized) return;

        cellGrid.Initialize();
        geometry.Initialize(cellGrid.CellRadius, boardView.bounds);

        initialized = true;
    }

    /// <summary>
	/// Resets the board to empty cells.
	/// </summary>
    private void Reset()
    {
        cellGrid.Reset();
        logic.ResetBoard(); // validate in logic
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
        //grid[index.x, index.y].SetColor(color);                       // render the player on the board
        cellGrid.SetCell(index.x, index.y, color);

        BoardLogic.PlayResult result;
        if (!logic.TryPlacePlayer(index.x, index.y, color, out result))     // run the board logic
        {
            Debug.LogError($"Ran play with invalid index or color: {index}, {color}");
        }

        if (result.FullBoard) EventBus.Publish(new GameOverEvent());                    // activate a game over

        // set full rows to empty cells
        if (result.ClearRow) cellGrid.ClearRow(index.x);
        if (result.ClearCol) cellGrid.ClearColumn(index.y);
        if (result.ClearRDiag) cellGrid.ClearRightDiagonal();
        if (result.ClearLDiag) cellGrid.ClearLeftDiagonal();

        if (result.Points > 0) EventBus.Publish(new WinEvent { Points = result.Points });
        EventBus.Publish(new TurnCompletedEvent());
    }

    /// <summary>
    /// Attempts to calculate the nearest valid board position for the player.
    /// </summary>
    /// <param name="position">World position to evaluate.</param>
    /// <param name="newPosition">
    /// The corresponding board-aligned world position if the location is valid.
    /// </param>
	/// <param name="index">
    /// The corresponding board index if the location is valid.
    /// </param>
    /// <returns>
    /// <c>true</c> if the position maps to a valid board cell; otherwise <c>false</c>.
    /// </returns>
    private bool TryGetPlayerPosition(Vector3 position, CellColor color, out Vector3 newPosition, out Vector2Int index)
    {
        index = geometry.TransformToBoardIndex(position);

        if (!logic.ValidCell(index.x, index.y, color))
        {
            newPosition = Vector3.zero;
            return false;
        }

        newPosition = geometry.BoardIndexToTransform(index);
        return true;
    }

    private bool TrySetShadow(Vector3 position, ref Vector2 min, ref Vector2 max, ref Vector2Int shadow)
    {
        if (!TryGetPlayerPosition(position, CellColor.WildCard, out Vector3 cellPosition, out shadow)) return false;

        cellGrid.SetCell(shadow.x, shadow.y, CellColor.Shadow);

        // update the bounds
        min.x = cellPosition.x - cellGrid.CellRadius;
        max.x = cellPosition.x + cellGrid.CellRadius;
        min.y = cellPosition.y - cellGrid.CellRadius;
        max.y = cellPosition.y + cellGrid.CellRadius;

        return true;
    }

    // ================================
    // Coroutines
    // ================================

    IEnumerator ShadowHover(Transform playerTransform)
    {
        Vector2 min, max;

        min.x = boardView.bounds.min.x; // set default x to the board
        max.x = boardView.bounds.max.x;

        min.y = playerTransform.position.y;
        max.y = boardView.bounds.min.y; // set default y to the bottom of the board

        Vector2Int shadow = Vector2Int.zero;
        while (true)
        {
            Vector3 position = playerTransform.position;
            float x = playerTransform.position.x;
            float y = playerTransform.position.y;

            if ((x < min.x || x > max.x || y < min.y || y > max.y))  // out of bounds
            {
                cellGrid.SetCell(shadow.x, shadow.y, CellColor.Empty);  // reset to empty

                if (!TrySetShadow(position, ref min, ref max, ref shadow))
                {
                    max.y = boardView.bounds.min.y; // default to retry each loop
                }
            }

            yield return null;
        }
    }

}
