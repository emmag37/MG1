using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// add require components
public class BoardView : MonoBehaviour
{
    // ================================
    // Constants
    // ================================
    private const int RowSize = GameConstants.RowSize;

    // ================================
    // Public Fields
    // ================================

    public Bounds BoardBounds => background.bounds;

    // ================================
    // Inspector Fields
    // ================================
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private GridView gridView;

    [SerializeField] private ScoreAnimation scoreAnimation;

    // ================================
    // Private Fields
    // ================================
    private BoardGeometry geometry;
    private BoardController boardController;

    private PieceRegistry pieceRegistry;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(background != null, "Background not set in board view");
    }

    void Awake()
    {
        pieceRegistry = GetComponent<PieceRegistry>();
        pieceRegistry.Initialize(BoardBounds);

        // old code
        boardController = GetComponent<BoardController>();
        geometry = new BoardGeometry();

        geometry.Initialize(BoardBounds);  // want to take the "radius" out of my board geometry
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


    // ================================
    // Public Methods
    // ================================
    // used by ghost preview

    public Vector2Int WorldToIndex(Vector3 position)
    {
        return geometry.TransformToBoardIndex(position);
    }

    public Vector3 IndexToWorld(Vector2Int index)
    {
        return geometry.BoardIndexToTransform(index);
    }

    public void SetCellColor(Vector2Int index, CellColor color)
    {
        pieceRegistry.SetPiece(index, color);
    }

    // ================================
    // Game State Event Handlers
    // ================================

    private void OnStartGame(StartGameEvent e)
    {
        background.gameObject.SetActive(true);
    }

    private void OnExitGame(ExitGameEvent e)
    {
        background.gameObject.SetActive(true);
    }

    private void OnGameOver(GameOverEvent e)
    {
        background.gameObject.SetActive(false);
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

    private void OnWin(WinEvent e)  // need to subscribe
    {
        StartCoroutine(WinAnimationRoutine(e));
    }

    private void OnBoardReset(ResetEvent e)
    {
        pieceRegistry.ResetPieces();
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
