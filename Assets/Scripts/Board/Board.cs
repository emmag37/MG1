using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PlayResult = BoardLogic.PlayResult;


[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(GhostPreview))]
public class Board : MonoBehaviour
{
    public bool InProgress => inProgress;

    // ================================
    // Events
    // ================================
    public event Action<int, int> FullBoard;        // score, highScore
    public event Action<Vector2Int> TutorialStepComplete;   // index of piece placed

    // ================================
    // Inspector Fields
    // ================================
    [SerializeField] private ScoreAnimation scoreAnimation;
    [SerializeField] private HUDController hUD;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject piecePrefab;

    // ================================
    // Private Fields
    // ================================

    private IAudio audioService;

    private GameData data;  // own its own instance of game data

    private BoardLogic logic = new BoardLogic();
    private PieceRegistry pieceRegistry;

    private SpriteRenderer spriteRenderer;
    private GhostPreview ghostPreview;

    private bool runTutorial;
    public bool inProgress;


    // ================================
    // Unity Lifecycle
    // ================================

    public void OnDestroy()
    {
        SubscribeToEvents(false);
    }

    // ================================
    // Initializers
    // ================================

    // initialize with the game load data
    public void Initialize(GameData data, int highScore, bool runTutorial, bool inProgress)
    {
        this.data = data;
        this.inProgress = inProgress;
        this.runTutorial = runTutorial;

        audioService = ServiceLocator.Get<IAudio>();

        // cache components
        spriteRenderer = GetComponent<SpriteRenderer>();
        ghostPreview = GetComponent<GhostPreview>();

        // initialize components
        BoardGeometry.Initialize(GameConstants.RowSize, GameConstants.RowSize, spriteRenderer.bounds);  // static class now

        ghostPreview.Initialize();
        scoreAnimation.Initialize();
        hUD.Initialize(highScore);

        pieceRegistry = new PieceRegistry(spriteRenderer.bounds, spawnPoint, piecePrefab);

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

        SubscribeToEvents(true);
    }

    public GameData Exit()
    {
        if (inProgress)
        {
            data.Score = hUD.Score;
            data.PlayerColors = pieceRegistry.Colors;

            logic.FillBoardData(data.Board);
        }
        return data;
    }

    // ================================
    // Public Methods
    // ================================

    // reset on game start
    public void PlayGame(bool restart = false)
    {
        if (runTutorial) TurnOffTutorial();

        if (restart) inProgress = false;

        // prepare a fresh game
        if (!inProgress)
        {
            Debug.Log("prepare fresh game");

            Reset();    // crash
            inProgress = true;

            SpawnPlayer();
        }

        audioService.PlayMusic(AudioType.GameMusic);
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
            pieceRegistry.SpawnNewPlayer(playerColor);
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
        if (!runTutorial)
            Debug.Assert(e.Color == pieceRegistry.Colors.PlayerColor, "player color mismatch");
        
        Vector2Int index = BoardGeometry.TransformToBoardIndex(e.PlayerPosition);
        int currentScore = ExecuteTurn(e.Color, index);

        if (currentScore == -1) return;

        if (runTutorial)
        {
            if (currentScore == 0) TutorialStepComplete?.Invoke(index);  // if points scored, invoke is timed to animation
            return;
        }

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
            TutorialStepComplete?.Invoke(index);
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

        if (runTutorial)
        {
            hUD.Reset();   // tutorial mode ALWAYS returns points scored, not current score
        }
        else if (result.FullBoard)
        {
            GameOver();
            return -1;
        }
        
        int currentScore = hUD.AddPoints(result.Points);
        if (result.Points > 0)
        {
            StartCoroutine(WinAnimationRoutine(result, index));
            audioService.PlaySoundEffect(AudioType.Win);
            ServiceLocator.Get<IVibration>().ShortVibration();
        }

        return currentScore;
    }

    private void SpawnPlayer()
    {
        CellColor nextColor = pieceRegistry.SpawnNewPlayer();

        hUD.SetPlayerPreview(nextColor);
    }

    private void Reset()
    {
        Debug.Assert(logic != null, "null logic");

        logic.ResetBoard();
        pieceRegistry.ResetPieces();
        hUD.Reset();
        data.Reset();
    }

    private void GameOver()
    {
        (int, int) finalScores = hUD.GameOver();
        inProgress = false;

        audioService.PlaySoundEffect(AudioType.GameOver);
        audioService.PlayMusic(AudioType.UIMusic);

        FullBoard?.Invoke(finalScores.Item1, finalScores.Item2);
    }

    private void TurnOffTutorial()
    {
        runTutorial = false;

        hUD.gameObject.SetActive(true); // need to put this somewhere else
    }

    // true to subscribe, false to unsubscribe
    private void SubscribeToEvents(bool subscribe)
    {
        Debug.Log($"subscribe to events: {subscribe}");

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
