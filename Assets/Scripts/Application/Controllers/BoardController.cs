using UnityEngine;
using System;
using System.Collections;

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

    private Coroutine ghostPreview;


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
        EventBus.Subscribe<GameOverEvent>(OnGameOver);

        // Player Events
        EventBus.Subscribe<PlayerReleasedEvent>(OnPlayerReleased);
        EventBus.Subscribe<PlayerDraggingEvent>(OnPlayerDragging);
    }

    void OnDisable()
    {
        // Game State Events
        EventBus.Unsubscribe<StartGameEvent>(OnStartGame);
        EventBus.Unsubscribe<ExitGameEvent>(OnExitGame);
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);

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

        if (!initialized)
            Initialize();
        else
            Reset();
    }

    private void OnExitGame(ExitGameEvent e)
    {
        // set inactive
        boardView.gameObject.SetActive(false);
        cellGrid.gameObject.SetActive(false);
    }

    private void OnGameOver(GameOverEvent e)
    {
        boardView.gameObject.SetActive(false);
        cellGrid.gameObject.SetActive(false);
    }


    // ================================
    // Player Event Handlers
    // ================================

    private void OnPlayerReleased(PlayerReleasedEvent e)
    {
        StopCoroutine(ghostPreview);

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
        ghostPreview = StartCoroutine(GhostPreviewLoop(e.PlayerTransform, e.Color));
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
        cellGrid.SetCell(index.x, index.y, color);

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
    private bool TryGetPlayerPosition(Vector3 position, CellColor color, out Vector3 newPosition, out  Vector2Int index)
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


    // ================================
    // Coroutines
    // ================================

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
                cellGrid.SetCell(index.x, index.y, ghostColor);
                ghostSet = false;
            }

            // set new ghost preview
            if (!ghostSet && valid)
            {
                index = newIndex;
                ghostColor = logic.GetCellColor(index.x, index.y);

                cellGrid.SetCell(index.x, index.y, CellColor.Shadow);
                ghostSet = true;
            }

            yield return null;
        }
    }

}
