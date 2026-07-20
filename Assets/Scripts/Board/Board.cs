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
    public event Action<int, int> FullBoard;        // score, highScore
    public event Action TutorialStepComplete;

    // ================================
    // Inspector Fields
    // ================================
    [SerializeField] private ScoreAnimation scoreAnimation;
    [SerializeField] private HUDController hUD;

    // ================================
    // Private Fields
    // ================================

    // own its own instance of game data
    private GameData data;

    private GameDataService gameDataService;        // remove
    private BoardLogic logic = new BoardLogic();

    private SpriteRenderer spriteRenderer;
    private PieceRegistry pieceRegistry;
    private GhostPreview ghostPreview;

    private bool runTutorial;
    private bool inProgress;

    // ================================
    // Initializers
    // ================================

    // initialize with the game load data
    public void Initialize(GameData data, int highScore, bool runTutorial, bool inProgress)
    {
        this.data = data;
        this.inProgress = inProgress;
        this.runTutorial = runTutorial;

        // cache components
        spriteRenderer = GetComponent<SpriteRenderer>();
        pieceRegistry = GetComponent<PieceRegistry>();
        ghostPreview = GetComponent<GhostPreview>();

        // initialize components
        BoardGeometry.Initialize(GameConstants.RowSize, GameConstants.RowSize, spriteRenderer.bounds);  // static class now
        ghostPreview.Initialize();
        hUD.Initialize(highScore);
        pieceRegistry.Initialize(spriteRenderer.bounds);    // add values to initialize the active game state

        // load systems
        if (inProgress)
        {
            hUD.LoadGame(data.Score, data.PlayerColors.NextColor);
            pieceRegistry.LoadGame(data.PlayerColors, data.Board.Cells);
            logic.AddCellsToBoard(data.Board.Cells);
        }

        if (runTutorial)
        {
            hUD.gameObject.SetActive(false);
        }

        // subscribe
        SubscribeToEvents(runTutorial || gameDataService.InProgress);
    }


    // close, or exit, or end or something

    // ================================
    // Public Methods
    // ================================

    // reset on game start
    public void PlayGame(bool restart = false)
    {
        if (runTutorial) TurnOffTutorial();

        if (restart) SetInProgress(false);

        // prepare a fresh game
        if (!gameDataService.InProgress)
        {
            Debug.Log("prepare fresh game");

            Reset();
            SetInProgress(true);

            SpawnPlayer();
        }

        EventBus.Publish(new StartGameEvent()); // for the audio
    }

    public void PauseGame(bool pause)
    {
        pieceRegistry.PausePlayer(pause);
    }

    public void StartTutorialStep(CellColor playerColor, (int, int)[] liveZone, IReadOnlyList<CellEntry> cells = null)
    {
        if (playerColor == CellColor.Empty)
        {
            Reset();
        }
        else
        {
            PlayerColors colors = new PlayerColors { PlayerColor = playerColor, NextColor = CellColor.Empty };    // no next for tutorial
            pieceRegistry.SpawnNewPlayer(colors);
        }

        logic.AddLiveZone(liveZone);

        if (cells != null)
        {
            logic.AddCellsToBoard(cells);
            pieceRegistry.LoadBoardPieces(cells);
        }

    }

    // ================================
    // Event Handlers
    // ================================

    // handles connection between logic and piece registry, crux that initiates a turn
    private void OnPlayerReleased(PlayerReleasedEvent e)
    {
        Debug.Assert(!runTutorial && e.Color == gameDataService.PlayerColors.PlayerColor, "player color mismatch");
        
        Vector2Int index = BoardGeometry.TransformToBoardIndex(e.PlayerPosition);
        int currentScore = ExecuteTurn(e.Color, index);

        if (currentScore == -1) return;

        if (runTutorial)
        {
            if (currentScore == 0) TutorialStepComplete?.Invoke();  // if points scored, invoke is timed to animation
            return;
        }

        gameDataService.SetScore(currentScore);
        gameDataService.AddPieceToBoard(index, e.Color);

        SpawnPlayer();
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
    
    private IEnumerator WinAnimationRoutine(PlayResult piecesToClear, Vector2Int index)
    {
        // clear piece coroutine in piece registry
        yield return pieceRegistry.PopPieces(piecesToClear, index);

        // run score animation
        if (runTutorial)
        {
            TutorialStepComplete?.Invoke();
        }
        else
        {
            Vector3 playerPos = BoardGeometry.BoardIndexToTransform(index);
            scoreAnimation.AnimateScore(piecesToClear.Points, playerPos);
        }
    }

    // ================================
    // Private Functions
    // ================================

    // returns the current score following the turn
    // main orchestration logic that should always be in this script
    private int ExecuteTurn(CellColor color, Vector2Int index)
    {
        if (!logic.TryPlacePlayer(index.x, index.y, color, out PlayResult result))
        {
            pieceRegistry.ReturnPlayerToStart();
            return -1;
        }

        Vector3 newPosition = BoardGeometry.BoardIndexToTransform(index);
        pieceRegistry.PlacePlayer(newPosition, index);

        if (result.FullBoard)
        {
            GameOver();
            return -1;
        }

        if (runTutorial) hUD.Reset();   // tutorial mode ALWAYS returns points scored, not current score
        
        int currentScore = hUD.AddPoints(result.Points);
        if (result.Points > 0)
        {
            StartCoroutine(WinAnimationRoutine(result, index));
            EventBus.Publish(new WinEvent());   // keep for audio manager
        }

        return currentScore;
    }

    private void SpawnPlayer()
    {
        PlayerColors colors = pieceRegistry.SpawnNewPlayer();

        hUD.SetPlayerPreview(colors.NextColor);
        gameDataService.SetPlayerColors(colors);
    }

    private void Reset()
    {
        Debug.Assert(logic != null, "null logic");

        logic.ResetBoard();
        pieceRegistry.ResetPieces();
        hUD.Reset();
        gameDataService.Reset();
    }

    private void GameOver()
    {
        (int, int) finalScores = hUD.GameOver();
        SetInProgress(false);

        FullBoard?.Invoke(finalScores.Item1, finalScores.Item2);
        EventBus.Publish(new GameOverEvent());
    }

    private void TurnOffTutorial()
    {
        runTutorial = false;

        SubscribeToEvents(false);
        hUD.gameObject.SetActive(true); // need to put this somewhere else
    }

    private void SetInProgress(bool inProgress)
    {
        gameDataService.InProgress = inProgress;
        SubscribeToEvents(inProgress);
    }

    // true to subscribe, false to unsubscribe
    private void SubscribeToEvents(bool subscribe)
    {
        if (subscribe)
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
