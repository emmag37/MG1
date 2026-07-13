using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PlayResult = BoardLogic.PlayResult;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PieceRegistry))]
[RequireComponent(typeof(GhostPreview))]
public class Board : MonoBehaviour
{
    // ================================
    // Events
    // ================================
    public event Action<bool, int, (int, int)> TurnCompleted;

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
    private PieceRegistry pieceRegistry;
    private GhostPreview ghostPreview;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        // cache components
        spriteRenderer = GetComponent<SpriteRenderer>();
        pieceRegistry = GetComponent<PieceRegistry>();
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
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<StartGameEvent>(OnStartGame);
        EventBus.Unsubscribe<ExitGameEvent>(OnExitGame);
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);

        EventBus.Unsubscribe<PlayerReleasedEvent>(OnPlayerReleased);
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

        pieceRegistry.SpawnNewPlayer();
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

    // handles connection between logic and piece registry, crux that initiates a turn
    private void OnPlayerReleased(PlayerReleasedEvent e)
    {
        Vector2Int index = geometry.TransformToBoardIndex(e.PlayerPosition);

        // run the board logic - returns early if invalid index
        if (!logic.TryPlacePlayer(index.x, index.y, e.Color, out PlayResult result))     
        {
            pieceRegistry.ReturnPlayerToStart();
            return;
        }

        Vector3 newPosition = geometry.BoardIndexToTransform(index);
        pieceRegistry.PlacePlayer(newPosition, index);

        // full board check

        if (result.Points > 0)
        {
            // animation sequence, also removes the instances from the board
            StartCoroutine(WinAnimationRoutine(result, index));

            // update points

            // keep for audio manager
            EventBus.Publish(new WinEvent());
        }

        pieceRegistry.SpawnNewPlayer();

        // save the turn, players, points, all of it to one save method

        TurnCompleted?.Invoke(result.FullBoard, result.Points, (index.x, index.y)); // remove this
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
    IEnumerator WinAnimationRoutine(PlayResult piecesToClear, Vector2Int index)
    {
        // clear piece coroutine in piece registry
        yield return pieceRegistry.PopPieces(piecesToClear, index);

        // run score animation
        Vector3 playerPos = geometry.BoardIndexToTransform(index);
        scoreAnimation.AnimateScore(piecesToClear.Points, playerPos);
    }
}
