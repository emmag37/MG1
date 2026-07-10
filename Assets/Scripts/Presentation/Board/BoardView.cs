using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PieceRegistry))]
public class BoardView : MonoBehaviour
{
    // ================================
    // Constants
    // ================================
    private const int RowSize = GameConstants.RowSize;

    // ================================
    // Public Fields
    // ================================

    public Bounds BoardBounds => spriteRenderer.bounds; // this is now on sprite renderer

    // ================================
    // Inspector Fields
    // ================================
    [SerializeField] private ScoreAnimation scoreAnimation;

    // ================================
    // Private Fields
    // ================================
    private BoardGeometry geometry;
    private BoardController boardController;

    private PieceRegistry pieceRegistry;
    private SpriteRenderer spriteRenderer;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        pieceRegistry = GetComponent<PieceRegistry>();
        spriteRenderer.GetComponent<SpriteRenderer>();

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

    private void OnWin(WinEvent e)  // need to subscribe
    {
        StartCoroutine(WinAnimationRoutine(e));
    }

    private void OnBoardReset(ResetEvent e)
    {
        pieceRegistry.ResetPieces();
    }

    // this script needs to manage ghost preview
    

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
