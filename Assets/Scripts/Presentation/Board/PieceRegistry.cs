using UnityEngine;
using System.Collections;

// i'm going to rename this file to PieceRegistry
// needs to live on the board - initialized by the board, like grid view is now

// todo: add as component to board view
// todo: implement ghost preview

public class PieceRegistry : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject playerPrefab;


    // ==================================================
    // Private Fields
    // ==================================================

    private Piece playerPiece;
    private Bounds playerBounds;

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

    public void SetPiece(Vector2Int index, CellColor color)
    {
        int idx = TwoDimToFlatIndex(index);
        Debug.Assert(pieces[idx], "Attempted to set null piece");

        pieces[idx].SetColor(color);
    }

    public void ResetPieces()
    {
        foreach (Piece piece in pieces)
        {
            if (piece) Destroy(piece.gameObject);
        }
    }

    // coroutine for pieces animation
    public IEnumerator PopPieces(WinEvent e)
    {
        // pop the pieces in rows

        // pop the player at index

        yield return null;
    }


    // ==================================================
    // Event Bus Methods
    // ==================================================

    private void OnSpawnPlayer(SpawnPlayerEvent e)
    {
        Debug.Assert(playerPiece == null, "Tried to instantiate a player when one already exists");

        playerPiece = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<Piece>();
        playerPiece.Initialize(e.Color, playerBounds);
    }

    private void OnPlacePlayer(PlacePlayerEvent e)
    {
        Debug.Log("Add active player to registry");

        playerPiece.PlacePlayer(e.PlayerPosition);

        int idx = TwoDimToFlatIndex(e.Index);
        pieces[idx] = playerPiece;
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

}
