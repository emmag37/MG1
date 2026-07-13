using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// rename to board - this is the root object for all other board components

// todo: fix board geometry to no longer hard code anything
    // also, this should be the only script accessing its values

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PieceRegistry))]
[RequireComponent(typeof(GhostPreview))]
public class BoardView : MonoBehaviour
{
    // ================================
    // Inspector Fields
    // ================================
    [SerializeField] private ScoreAnimation scoreAnimation;

    // ================================
    // Private Fields
    // ================================
    private BoardGeometry geometry;
    private SpriteRenderer spriteRenderer;

    private BoardController boardController;
    private PieceRegistry pieceRegistry;
    private GhostPreview ghostPreview;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        // cache components
        pieceRegistry = GetComponent<PieceRegistry>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boardController = GetComponent<BoardController>();
        ghostPreview = GetComponent<GhostPreview>();

        // initialize components
        pieceRegistry.Initialize(spriteRenderer.bounds);
        geometry = new BoardGeometry(GameConstants.RowSize, GameConstants.RowSize, spriteRenderer.bounds);
        ghostPreview.Initialize(geometry);

        // subscribe to events
        ghostPreview.TryGhostPreview += HandleGhostPreview;
    }

    void OnEnable()
    {
        EventBus.Subscribe<StartGameEvent>(OnStartGame);
        EventBus.Subscribe<ExitGameEvent>(OnExitGame);
        EventBus.Subscribe<GameOverEvent>(OnGameOver);

        EventBus.Subscribe<PlayerReleasedEvent>(OnPlayerReleased);
        EventBus.Subscribe<WinEvent>(OnWin);
        EventBus.Subscribe<ResetEvent>(OnBoardReset);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<StartGameEvent>(OnStartGame);
        EventBus.Unsubscribe<ExitGameEvent>(OnExitGame);
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);

        EventBus.Unsubscribe<PlayerReleasedEvent>(OnPlayerReleased);
        EventBus.Unsubscribe<WinEvent>(OnWin);
        EventBus.Unsubscribe<ResetEvent>(OnBoardReset);
    }

    void OnDestroy()
    {
        ghostPreview.TryGhostPreview -= HandleGhostPreview;
    }


    // ================================
    // Public Methods
    // ================================

    public void SetCellColor(Vector2Int index, CellColor color)
    {
        pieceRegistry.TrySetPieceColor(index, color);
    }

    // ================================
    // Game State Event Handlers
    // ================================

    private void OnStartGame(StartGameEvent e)
    {
        spriteRenderer.gameObject.SetActive(true);
    }

    private void OnExitGame(ExitGameEvent e)
    {
        spriteRenderer.gameObject.SetActive(true);
    }

    private void OnGameOver(GameOverEvent e)
    {
        spriteRenderer.gameObject.SetActive(false);
    }


    // ================================
    // Player/Board Event Handlers
    // ================================

    private void OnPlayerReleased(PlayerReleasedEvent e)
    {
        Vector2Int index = geometry.TransformToBoardIndex(e.PlayerPosition);
        Vector3 newPosition = geometry.BoardIndexToTransform(index);

        boardController.TryPlacePlayer(index, e.Color, newPosition);
    }

    // on player dragging
    // start ghost preview routine

    private void OnWin(WinEvent e)  // need to subscribe
    {
        StartCoroutine(WinAnimationRoutine(e));
    }

    private void OnBoardReset(ResetEvent e)
    {
        pieceRegistry.ResetPieces();
    }

    private void HandleGhostPreview(Vector2Int index, CellColor color)
    {
        boardController.TryGhostPreview(index, color, (color != CellColor.Empty));
    }
    

    // ================================
    // Coroutines
    // ================================

    // need to streamline this
    IEnumerator WinAnimationRoutine(WinEvent piecesToClear)
    {
        // clear piece coroutine in piece registry
        yield return pieceRegistry.PopPieces(piecesToClear);

        // run score animation
        Vector3 playerPos = geometry.BoardIndexToTransform(piecesToClear.Index);
        scoreAnimation.AnimateScore(piecesToClear.Points, playerPos);
    }
}
