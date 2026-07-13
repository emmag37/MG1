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
    private BoardLogic logic;

    private SpriteRenderer spriteRenderer;

    private BoardController boardController;        // just try to remove this
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
        geometry = new BoardGeometry(GameConstants.RowSize, GameConstants.RowSize, spriteRenderer.bounds);
        logic = new BoardLogic();
        pieceRegistry.Initialize(spriteRenderer.bounds);
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
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<StartGameEvent>(OnStartGame);
        EventBus.Unsubscribe<ExitGameEvent>(OnExitGame);
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);

        EventBus.Unsubscribe<PlayerReleasedEvent>(OnPlayerReleased);
        EventBus.Unsubscribe<WinEvent>(OnWin);
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

    public void Reset()
    {
        logic.ResetBoard();
        pieceRegistry.ResetPieces();
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

    // handles connection between logic and piece registry
    // crux that initiates a turn
    private void OnPlayerReleased(PlayerReleasedEvent e)
    {
        Vector2Int index = geometry.TransformToBoardIndex(e.PlayerPosition);
        if (!logic.ValidCell(index.x, index.y, e.Color))
        {
            pieceRegistry.ReturnPlayerToStart();
            return;
        }

        Vector3 newPosition = geometry.BoardIndexToTransform(index);
        pieceRegistry.PlacePlayer(newPosition, index);

        //RunPlay(index, color);
    }

    private void OnWin(WinEvent e)
    {
        StartCoroutine(WinAnimationRoutine(e));
    }

    private void HandleGhostPreview(Vector2Int index, CellColor color)
    {
        if (logic.ValidCell(index.x, index.y, color))
        {
            ghostPreview.SetPreview(index);
        }
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
