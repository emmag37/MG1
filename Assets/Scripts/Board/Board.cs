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

    // ==================================================
    // Events
    // ==================================================
    public event Action<int, int> FullBoard;                // score, highScore
    public event Action<Vector2Int> TutorialStepComplete;   // index of piece placed

    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private ScoreAnimation scoreAnimation;
    [SerializeField] private HUDController hUD;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject piecePrefab;

    // ==================================================
    // Private Fields
    // ==================================================
    private IAudio audioService;

    private GameData data = new GameData();

    private BoardLogic logic = new BoardLogic();
    private PieceRegistry pieceRegistry;

    private GhostPreview ghostPreview;

    private bool runTutorial = false;
    public bool inProgress = false;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    public void OnValidate()
    {
        Debug.Assert(scoreAnimation != null, "[Board] Score Animation is null");
        Debug.Assert(hUD != null, "[Board] HUD is null");
        Debug.Assert(spawnPoint != null, "[Board] Spawn Point is null");
        Debug.Assert(piecePrefab != null, "[Board] Piece Prefab is null");
    }

    public void OnDestroy()
    {
        SubscribeToEvents(false);
    }

    // ==================================================
    // Initializers
    // ==================================================
    
    public void Initialize(int highScore)
    {
        // cache components
        audioService = ServiceLocator.Get<IAudio>();                        
        ghostPreview = GetComponent<GhostPreview>();                        
        SpriteRenderer boardSprite = GetComponent<SpriteRenderer>();        

        // run calculations
        PieceData pieceData = CalculatePieceData(spawnPoint.position, boardSprite.bounds);              
        BoardGeometry.Initialize(GameConstants.RowSize, GameConstants.RowSize, boardSprite.bounds);     

        // initialize components
        ghostPreview.Initialize();      
        scoreAnimation.Initialize();    
        hUD.Initialize(highScore);      

        var pool = new GameObjectPool<Piece, PieceData>(GameConstants.NumberCells + 1, piecePrefab, pieceData);
        pieceRegistry = new PieceRegistry(pool);        

        SubscribeToEvents(true);
    }

    public void Load(GameData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data), "[Board] Requires non-null Game Data for Load");

        this.data = data;       
        
        if (!pieceRegistry.LoadGame(data.PlayerColors, data.Board.Cells))
            throw new GameLoadException("[Board] Error rendering saved game state");

        hUD.LoadGame(data.Score, data.PlayerColors.NextColor);          
        logic.AddCellsToBoard(data.Board.Cells);

        inProgress = true;          // only set if no exceptions thrown
    }

    public void RunTutorial()
    {
        runTutorial = true;
        hUD.gameObject.SetActive(false);
    }

    // ensures non-null return value
    public GameData GetGameData()
    {
        if (data == null)
            throw new InvalidOperationException("[Board] Cannot mutate null data to return");

        if (!inProgress)
            Debug.LogWarning("[Board] Requested game data while in inactive state");

        data.Score = hUD.Score;
        data.PlayerColors = pieceRegistry.Colors;
		data.Board = logic.FillBoardData();

        return data;
    }

    // ================================
    // Public Methods
    // ================================

    // reset on game start
    public void PlayGame(bool restart = false)
    {
        if (runTutorial)
            TurnOffTutorial();

        if (restart)
            inProgress = false;

        // prepare a fresh game
        if (!inProgress)
        {
            Reset();
            inProgress = true;

            SpawnPlayer();
        }

        PauseGame(false);
        audioService.PlayMusic((int)AudioType.GameMusic);
    }

    public void PauseGame(bool pause)
    {
        pieceRegistry.PausePlayer(pause);
    }

    public void StartTutorialStep(CellColor playerColor, (int, int)[] liveZone, IReadOnlyList<CellEntry> cells = null)
    {
        if (playerColor == CellColor.Empty)
            Reset();
        else
            pieceRegistry.TrySpawnNewPlayer(out _, playerColor);

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
            audioService.PlaySoundEffect((int)AudioType.Win);
            ServiceLocator.Get<IVibration>().ShortVibration();
        }

        return currentScore;
    }

    private void SpawnPlayer()
    {
        pieceRegistry.TrySpawnNewPlayer(out CellColor nextColor);
        hUD.SetPlayerPreview(nextColor);
    }

    private void Reset()
    {
        Debug.Assert(logic != null, "null logic");

        data.Reset();
        logic.ResetBoard();
        pieceRegistry.ResetPieces();
        hUD.Reset();
    }

    private void GameOver()
    {
        (int, int) finalScores = hUD.GetScores();
        inProgress = false;

        audioService.PlaySoundEffect((int)AudioType.GameOver);
        audioService.PlayMusic((int)AudioType.UIMusic);

        FullBoard?.Invoke(finalScores.Item1, finalScores.Item2);
    }

    private void TurnOffTutorial()
    {
        runTutorial = false;
        hUD.gameObject.SetActive(true);
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

    private PieceData CalculatePieceData(Vector3 spawnPos, Bounds boardBounds)
    {
        spawnPos = Scaler.CalculateScaledYPos(spawnPos);

        Vector3 min = boardBounds.min;
        min.y = spawnPos.y;

        Bounds playerBounds = boardBounds;
        playerBounds.SetMinMax(min, playerBounds.max);

        return new PieceData { boundaries = playerBounds, spawnPoint = spawnPos };
    }
}
