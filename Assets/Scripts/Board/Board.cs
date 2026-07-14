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
    private BoardLogic logic;

    private SpriteRenderer spriteRenderer;
    private PieceRegistry pieceRegistry;
    private GhostPreview ghostPreview;

    private bool subscribed;


    // ================================
    // Initializers
    // ================================

    // initialize with the game load data
    public void Initialize(GameDataService dataService)
    {
        gameData = dataService;
        subscribed = false;

        // cache components
        spriteRenderer = GetComponent<SpriteRenderer>();
        pieceRegistry = GetComponent<PieceRegistry>();
        ghostPreview = GetComponent<GhostPreview>();

        // initialize components
        BoardGeometry.Initialize(GameConstants.RowSize, GameConstants.RowSize, spriteRenderer.bounds);  // static class now

        logic = new BoardLogic();   // add an overloaded constructor

        pieceRegistry.Initialize(spriteRenderer.bounds, dataService);    // add values to initialize the active game state
        //ghostPreview.Initialize(geometry);  // edit this
        hUD.Initialize(dataService);

        // subscribe to events if active game
    }

    // ================================
    // Public Methods
    // ================================

    // reset on game start
    public void FreshGame()
    {
        Reset();

        if (!subscribed)    // should never double-subscribe
        {
            ghostPreview.TryGhostPreview += HandleGhostPreview;
            EventBus.Subscribe<PlayerReleasedEvent>(OnPlayerReleased);
            subscribed = true;
        }

        pieceRegistry.SpawnNewPlayer();
    }

    public void PauseGame(bool pause)
    {
        pieceRegistry.PausePlayer(pause);
    }

    public void Reset()
    {
        logic.ResetBoard();
        pieceRegistry.ResetPieces();
        hUD.Reset();
        gameData.ResetGame();
    }

    public void SetCellColor(Vector2Int index, CellColor color)
    {
        pieceRegistry.TrySetPieceColor(index, color);
    }

    // ================================
    // Player/Board Event Handlers
    // ================================

    // handles connection between logic and piece registry, crux that initiates a turn
    private void OnPlayerReleased(PlayerReleasedEvent e)
    {
        Vector2Int index = BoardGeometry.TransformToBoardIndex(e.PlayerPosition);

        // run the board logic - returns early if invalid index
        if (!logic.TryPlacePlayer(index.x, index.y, e.Color, out PlayResult result))     
        {
            pieceRegistry.ReturnPlayerToStart();
            return;
        }

        Vector3 newPosition = BoardGeometry.BoardIndexToTransform(index);
        pieceRegistry.PlacePlayer(newPosition, index);

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

        CellColor nextColor = pieceRegistry.SpawnNewPlayer();
        hUD.SetPlayerPreview(nextColor);

        gameData.SaveTurn(hUD.Score, index);
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
        Vector3 playerPos = BoardGeometry.BoardIndexToTransform(index);
        scoreAnimation.AnimateScore(piecesToClear.Points, playerPos);
    }

    // ================================
    // Private Functions
    // ================================

    private void GameOver()
    {
        hUD.GameOver();
        FullBoard?.Invoke();

        // unsubscribe from events
        ghostPreview.TryGhostPreview -= HandleGhostPreview;
        EventBus.Unsubscribe<PlayerReleasedEvent>(OnPlayerReleased);
        subscribed = false;
    }

}
