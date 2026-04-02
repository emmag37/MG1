using UnityEngine;
using System.Collections;

public class GhostPreview : MonoBehaviour
{
    // ================================
    // Private Fields
    // ================================
    private BoardView boardView;
    private BoardController boardController;

    private Coroutine preview;

    private bool previewSet;
    private Vector2Int previewIndex;
    private CellColor originalColor;

    
    // ================================
    // Unity Lifecycle
    // ================================

    void Awake()
    {
        boardView = GetComponent<BoardView>();
        boardController = GetComponent<BoardController>();

        previewSet = false;
    }

    void OnEnable()
    {
        // Player Events
        EventBus.Subscribe<PlayerDraggingEvent>(OnPlayerDragging);
        EventBus.Subscribe<PlayerReleasedEvent>(OnPlayerRelease);
    }

    void OnDisable()
    {
        // Player Events
        EventBus.Unsubscribe<PlayerDraggingEvent>(OnPlayerDragging);
        EventBus.Unsubscribe<PlayerReleasedEvent>(OnPlayerRelease);
    }


    // ================================
    // Event Bus Handlers
    // ================================

    private void OnPlayerDragging(PlayerDraggingEvent e)
    {
        // start the ghost preview
        preview = StartCoroutine(PreviewLoop(e.PlayerTransform, e.Color));

        EventBus.Subscribe<GhostPreviewEvent>(OnPreview);
    }

    private void OnPlayerRelease(PlayerReleasedEvent e)
    {
        // stop the ghost preview
        StopCoroutine(preview);

        previewSet = false;

        EventBus.Unsubscribe<GhostPreviewEvent>(OnPreview);
    }

    private void OnPreview(GhostPreviewEvent e)
    {
        previewSet = true;

        originalColor = e.OriginalColor;
        previewIndex = e.Index;
    }


    // ================================
    // Coroutine
    // ================================

    IEnumerator PreviewLoop(Transform player, CellColor color)
    {
        while (true)
        {
            if (player == null)
                yield break;

            Vector3 position = player.position;     // ERROR - event passing?
            Vector2Int index = boardView.WorldToIndex(position);

            // try a new preview only when the index changes
            if (previewSet && index != previewIndex)
            {
                ClearPreview();
                boardController.TryGhostPreview(index, color);
            }
            else if (!previewSet)
            {
                boardController.TryGhostPreview(index, color);
            }

            yield return null;
        }
    }


    // ================================
    // Private Methods
    // ================================

    private void ClearPreview()
    {
        previewSet = false;

        boardView.SetCellColor(previewIndex, originalColor);
    }

}
