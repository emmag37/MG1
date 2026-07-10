using UnityEngine;
using System.Collections;

// script is clean and ready to remove event bus

public class PieceRegistry : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject piecePrefab;

    // ==================================================
    // Private Fields
    // ==================================================
    private Piece playerPiece;
    private Bounds playerBounds;

    // eventually turn this into an object pool to reuse objects
    private Piece[] pieces = new Piece[GameConstants.RowSize * GameConstants.RowSize];


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnEnable()
    {
        EventBus.Subscribe<SpawnPlayerEvent>(OnSpawnPlayer);
        EventBus.Subscribe<PlacePlayerEvent>(OnPlacePlayer);
        EventBus.Subscribe<DestroyPlayerEvent>(OnDestroyPlayer);

        EventBus.Subscribe<PauseGameEvent>(OnPausePlayer);
        EventBus.Subscribe<ResumeGameEvent>(OnResumePlayer);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<SpawnPlayerEvent>(OnSpawnPlayer);
        EventBus.Unsubscribe<PlacePlayerEvent>(OnPlacePlayer);
        EventBus.Unsubscribe<DestroyPlayerEvent>(OnDestroyPlayer);

        EventBus.Unsubscribe<PauseGameEvent>(OnPausePlayer);
        EventBus.Unsubscribe<ResumeGameEvent>(OnResumePlayer);
    }


    // ==================================================
    // Initializer
    // ==================================================

    public void Initialize(Bounds boardBounds)
    {
        Vector3 min = boardBounds.min;
        min.y = spawnPoint.position.y;

        playerBounds = boardBounds;
        playerBounds.SetMinMax(min, playerBounds.max);
    }


    // ==================================================
    // Public Methods
    // ==================================================

    public bool TrySetPieceColor(Vector2Int index, CellColor color)
    {
        int idx = TwoDimToFlatIndex(index);
        if (pieces[idx] == null) return false;

        pieces[idx].SetColor(color);
        return true;
    }

    public void ResetPieces()
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            DestroyPieceAt(i);
        }
    }

    // Coroutine for popping pieces animation
        // note: the pieces currently destroy themselves after animation, would like to add object pool for later
    public IEnumerator PopPieces(WinEvent e)
    {
        int cleared = 0;
        int total = (e.Row ? 4 : 0) + (e.Column ? 4 : 0) + (e.RightDiag ? 4 : 0) + (e.LeftDiag ? 4 : 0) + 1;

        void OnPopFinished(Piece piece)
        {
            cleared++;
            piece.PopFinished -= OnPopFinished;
        }

        // helper
        void PopPieceAt(int row, int col)
        {
            Piece piece = RemovePieceAt(row, col);
            piece.PopFinished += OnPopFinished;
            piece.Pop();
        }

        // pop the pieces in filled lines EXCEPT player
        for (int i = 0; i < GameConstants.RowSize; i++)
        {
            if (e.Row && i != e.Index.y)    // i is not the player
                PopPieceAt(e.Index.x, i);

            if (e.Column && i != e.Index.x)
                PopPieceAt(i, e.Index.y);

            if (e.RightDiag && i != e.Index.x)
                PopPieceAt(i, i);

            if (e.LeftDiag && i != e.Index.x)   // start with top left
                PopPieceAt(i, GameConstants.RowSize - 1 - i);
        }

        Piece player = RemovePieceAt(e.Index.x, e.Index.y);
        player.PopFinished += OnPopFinished;
        player.Pop();

        yield return new WaitUntil(() => cleared >= total);
    }


    // ==================================================
    // Event Bus Methods
    // ==================================================

    private void OnSpawnPlayer(SpawnPlayerEvent e)
    {
        Debug.Assert(playerPiece == null, "Tried to instantiate a player when one already exists");

        playerPiece = Instantiate(piecePrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<Piece>();
        playerPiece.Initialize(e.Color, playerBounds);
    }

    private void OnPlacePlayer(PlacePlayerEvent e)
    {
        playerPiece.PlacePlayer(e.PlayerPosition);

        int idx = TwoDimToFlatIndex(e.Index);
        if (playerPiece.Color == CellColor.Mask)
            DestroyPieceAt(idx);

        pieces[idx] = playerPiece;

        playerPiece = null;
    }

    private void OnDestroyPlayer(DestroyPlayerEvent e)
    {
        Debug.Assert(playerPiece != null, "Tried to destroy non-existent player");

        Destroy(playerPiece.gameObject);
        playerPiece = null;
    }

    private void OnPausePlayer(PauseGameEvent e)
    {
        playerPiece.enabled = false;
    }

    private void OnResumePlayer(ResumeGameEvent e)
    {
        playerPiece.enabled = true;
    }

    // ==================================================
    // Private Methods
    // ==================================================

    private int TwoDimToFlatIndex(Vector2Int index)
    {
        return index.x * GameConstants.RowSize + index.y;     // x: row, y: column
    }

    private Vector2Int FlatToTwoDimIndex(int flatIndex)
    {
        return new Vector2Int(flatIndex / GameConstants.RowSize, flatIndex % GameConstants.RowSize);
    }

    private Piece RemovePieceAt(int row, int col)
    {
        int idx = TwoDimToFlatIndex(new Vector2Int(row, col));
        Piece piece = pieces[idx];
        pieces[idx] = null;

        return piece;
    }

    private void DestroyPieceAt(int index)
    {
        if (!pieces[index]) return;

        Destroy(pieces[index].gameObject);
        pieces[index] = null;
    }
}
