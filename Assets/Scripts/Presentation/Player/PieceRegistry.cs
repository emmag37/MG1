using UnityEngine;

// i'm going to rename this file to PieceRegistry
// needs to live on the board - initialized by the board, like grid view is now

public class PlayerSpawner : MonoBehaviour
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


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnEnable()
    {
        EventBus.Subscribe<SpawnPlayerEvent>(OnSpawnPlayer);
        EventBus.Subscribe<DestroyPlayerEvent>(OnDestroyPlayer);

        EventBus.Subscribe<PauseGameEvent>(OnPausePlayer);
        EventBus.Subscribe<ResumeGameEvent>(OnResumePlayer);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<SpawnPlayerEvent>(OnSpawnPlayer);
        EventBus.Unsubscribe<DestroyPlayerEvent>(OnDestroyPlayer);

        EventBus.Unsubscribe<PauseGameEvent>(OnPausePlayer);
        EventBus.Unsubscribe<ResumeGameEvent>(OnResumePlayer);
    }


    // ==================================================
    // Initializer
    // ==================================================

    public void Initialize(Bounds boardBounds)
    {
        SetPlayerBoundaries(boardBounds);
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

    private void OnDestroyPlayer(DestroyPlayerEvent e)
    {
        Debug.Assert(playerPiece != null, "Tried to destroy non-existent player");

        Destroy(playerPiece.gameObject);
        playerPiece = null;
    }

    // this way only the active player is paused - combine
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

    private void SetPlayerBoundaries(Bounds boardBounds)
    {
        playerBounds = boardBounds;

        Vector3 min = playerBounds.min;
        min.y = spawnPoint.position.y;

        playerBounds.SetMinMax(min, playerBounds.max);
    }

}
