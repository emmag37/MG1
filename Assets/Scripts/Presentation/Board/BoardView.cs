using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(background != null, "Background not set in board view");
    }

    void Awake()
    {
        // put the scaling for the board here? 

        boardController = GetComponent<BoardController>();
        geometry = new BoardGeometry();

        gridView.Initialize();
        geometry.Initialize(gridView.CellRadius, BoardBounds);
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
        gridView.SetCell(index, color);
    }

    // ================================
    // Game State Event Handlers
    // ================================

    private void OnStartGame(StartGameEvent e)
    {
        background.gameObject.SetActive(true);
        gridView.gameObject.SetActive(true);
    }

    private void OnExitGame(ExitGameEvent e)
    {
        background.gameObject.SetActive(true);
        gridView.gameObject.SetActive(true);
    }

    private void OnGameOver(GameOverEvent e)
    {
        background.gameObject.SetActive(false);
        gridView.gameObject.SetActive(false);
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
        List<Cell> cells = gridView.GetCellsToClear(e.Index, e.Row, e.Column, e.RightDiag, e.LeftDiag);
        Vector3 playerPos = geometry.BoardIndexToTransform(e.Index);

        StartCoroutine(WinAnimationRoutine(cells, e.Points, playerPos));
    }

    private void OnBoardReset(ResetEvent e)
    {
        gridView.ResetCells();
    }

    // ================================
    // Coroutines
    // ================================

    IEnumerator WinAnimationRoutine(List<Cell> cells, int points, Vector3 playerPos)
    {
        int cleared = 0;
        int total = cells.Count;

        void OnCellCleared(Cell cell)
        {
            cleared++;
            cell.PopFinished -= OnCellCleared;
        }

        // pop each cell
        foreach (Cell cell in cells)
        {
            cell.PopFinished += OnCellCleared;
            cell.Pop();
        }

        // wait for all cells to pop
        yield return new WaitUntil(() => cleared >= total);

        // run score animation
        scoreAnimation.AnimateScore(points, playerPos);
    }
}
