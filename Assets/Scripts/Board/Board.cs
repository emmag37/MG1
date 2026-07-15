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

    private bool inProgress;

    // ================================
    // Initializers
    // ================================

    // initialize with the game load data
    public void Initialize(InitFlag initInfo, GameDataService dataService)
    {
        gameData = dataService;
        inProgress = gameData.GetGameData().InProgress;

        // cache components
        spriteRenderer = GetComponent<SpriteRenderer>();
        pieceRegistry = GetComponent<PieceRegistry>();
        ghostPreview = GetComponent<GhostPreview>();

        // initialize components
        BoardGeometry.Initialize(GameConstants.RowSize, GameConstants.RowSize, spriteRenderer.bounds);  // static class now
        pieceRegistry.Initialize(spriteRenderer.bounds, dataService);    // add values to initialize the active game state
        ghostPreview.Initialize();
        hUD.Initialize(dataService);

        // in-progress only initialization
        IReadOnlyList<CellEntry> cells = null;
        if (inProgress)
        {
            SetInProgress(true);
            cells = dataService.GetGamePlayData().Board.Cells;
        }
        logic = new BoardLogic(cells);
    }

    // ================================
    // Public Methods
    // ================================

    // reset on game start
    public void PlayGame(bool restart = false)
    {
        if (restart) SetInProgress(false);

        // prepare a fresh game
        if (!inProgress)
        {
            Reset();
            if (!inProgress) SetInProgress(true);

            CellColor nextColor = pieceRegistry.SpawnNewPlayer().NextColor;
            hUD.SetPlayerPreview(nextColor);
        }

        EventBus.Publish(new StartGameEvent()); // for the audio
    }

    public void PauseGame(bool pause)
    {
        pieceRegistry.PausePlayer(pause);
    }

    public void StartTutorialStep(CellColor playerColor, (int, int)[] liveZone)
    {
        Debug.Log("start tutorial step");

        logic.AddLiveZone(liveZone);    // what behavior for null?

        var colors = new PlayerPicker.PlayerColors { Color = playerColor, NextColor = CellColor.Empty };    // no next for tutorial
        pieceRegistry.SpawnNewPlayer(colors);
    }

    // ================================
    // Player/Board Event Handlers
    // ================================

    // handles connection between logic and piece registry, crux that initiates a turn
    private void OnPlayerReleased(PlayerReleasedEvent e)
    {
        Debug.Log("on player released");

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

        int currentScore = hUD.AddPoints(result.Points);
        if (result.Points > 0)
        {
            StartCoroutine(WinAnimationRoutine(result, index));
            EventBus.Publish(new WinEvent());   // keep for audio manager
        }

        gameData.SaveTurn(currentScore, index);    // MUST save turn first, uses original stored colors

        CellColor nextColor = pieceRegistry.SpawnNewPlayer().NextColor;
        hUD.SetPlayerPreview(nextColor);

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

    private void Reset()
    {
        logic.ResetBoard();
        pieceRegistry.ResetPieces();
        hUD.Reset();
        gameData.ResetGame();
    }

    private void GameOver()
    {
        hUD.GameOver();
        SetInProgress(false);

        FullBoard?.Invoke();
        EventBus.Publish(new GameOverEvent());
    }

    private void SetInProgress(bool inProgress)
    {
        Debug.Log($"set in progress: {inProgress}");

        this.inProgress = inProgress;
        gameData.SetInProgress(inProgress);

        if (inProgress)
        {
            ghostPreview.TryGhostPreview += HandleGhostPreview;
            EventBus.Subscribe<PlayerReleasedEvent>(OnPlayerReleased);
        }
        else
        {
            ghostPreview.TryGhostPreview -= HandleGhostPreview;
            EventBus.Unsubscribe<PlayerReleasedEvent>(OnPlayerReleased);
        }
    }
}
