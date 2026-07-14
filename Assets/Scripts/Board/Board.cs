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
    public event Action FullBoard;

    // ================================
    // Inspector Fields
    // ================================
    [SerializeField] private ScoreAnimation scoreAnimation;
    [SerializeField] private HUDController hUD;

    // ================================
    // Private Fields
    // ================================
    private GameDataService gameData;

    private BoardGeometry geometry;
    private BoardLogic logic;

    private SpriteRenderer spriteRenderer;
    private PieceRegistry pieceRegistry;
    private GhostPreview ghostPreview;

    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnEnable()
    {
        EventBus.Subscribe<StartGameEvent>(OnStartGame);
        EventBus.Subscribe<PlayerReleasedEvent>(OnPlayerReleased);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<StartGameEvent>(OnStartGame);
        EventBus.Unsubscribe<PlayerReleasedEvent>(OnPlayerReleased);
    }

    void OnDestroy()
    {
        ghostPreview.TryGhostPreview -= HandleGhostPreview;
    }


    // ================================
    // Initializers
    // ================================

    public void Initialize(GameDataService dataService)
    {
        gameData = dataService;

        // cache components
        spriteRenderer = GetComponent<SpriteRenderer>();
        pieceRegistry = GetComponent<PieceRegistry>();
        ghostPreview = GetComponent<GhostPreview>();

        // initialize components
        geometry = new BoardGeometry(GameConstants.RowSize, GameConstants.RowSize, spriteRenderer.bounds);
        logic = new BoardLogic();
        pieceRegistry.Initialize(spriteRenderer.bounds);
        ghostPreview.Initialize(geometry);
        hUD.Initialize(dataService);

        // subscribe to events
        ghostPreview.TryGhostPreview += HandleGhostPreview;
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
        hUD.Reset();
    }

    public void PauseGame(bool pause)
    {
        pieceRegistry.PausePlayer(pause);
    }

    // ================================
    // Game State Event Handlers
    // ================================

    private void OnStartGame(StartGameEvent e)
    {
        var colors = pieceRegistry.SpawnNewPlayer();
        gameData.SavePlayerColors(colors.Color, colors.NextColor);
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

        // final piece - all game over events (besides UIManager and audio manager) run through here
        if (result.FullBoard)
        {
            GameOver();
            return;
        }

        if (result.Points > 0)
        {
            hUD.AddPoints(result.Points);
            StartCoroutine(WinAnimationRoutine(result, index));
            EventBus.Publish(new WinEvent());   // keep for audio manager
        }

        var playerColors = pieceRegistry.SpawnNewPlayer();
        hUD.SetPlayerPreview(playerColors.NextColor);

        gameData.SaveTurn(hUD.Score, index, playerColors);
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

    // ================================
    // Private Functions
    // ================================

    private void GameOver()
    {
        hUD.GameOver();
        Reset();
        FullBoard?.Invoke();
    }

}
