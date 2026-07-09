using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    // ================================
    // Inspector Fields
    // ================================

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private BoardView boardView;


    // ================================
    // Private Fields
    // ================================

    private PlayerView player;
    private Bounds playerBounds;


    // ================================
    // Unity Lifecycle
    // ================================

    void OnValidate()
    {
        Debug.Assert(spawnPoint != null, "Spawn point not set in composition root");
        Debug.Assert(playerPrefab != null, "Player prefab not set in composition root");

        Debug.Assert(boardView != null, "Board view not set in composition root");
    }

    void Start()
    {
        InitializePlayerBoundaries();
    }

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


    // ================================
    // Event Bus Methods
    // ================================

    private void OnSpawnPlayer(SpawnPlayerEvent e)
    {
        Debug.Assert(player == null, "Tried to instantiate a player when one already exists");

        player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<PlayerView>();
        player.Initialize(e.Color, playerBounds);
    }
    
    private void OnDestroyPlayer(DestroyPlayerEvent e)
    {
        Debug.Assert(player != null, "Tried to destroy non-existent player");

        Destroy(player.gameObject);
        player = null;
    }

    private void OnPausePlayer(PauseGameEvent e)
    {
        player.enabled = false;
    }

    private void OnResumePlayer(ResumeGameEvent e)
    {
        player.enabled = true;
    }


    // ================================
    // Private Methods
    // ================================

    private void InitializePlayerBoundaries()
    {
        playerBounds = boardView.BoardBounds;

        Vector3 min = playerBounds.min;
        min.y = spawnPoint.position.y;

        playerBounds.SetMinMax(min, playerBounds.max);
    }

}
