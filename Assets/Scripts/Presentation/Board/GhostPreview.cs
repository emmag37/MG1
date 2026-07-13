using UnityEngine;
using System.Collections;
using System;

// deprecate the GhostPreviewEvent on the event bus
public class GhostPreview : MonoBehaviour
{
    // ================================
    // Events
    // ================================
    public event Action<Vector2Int, CellColor> TryGhostPreview;

    // ================================
    // Private Fields
    // ================================
    [SerializeField] private SpriteRenderer shadowSprite;
    private BoardGeometry geometry;

    private Coroutine preview;
    private bool previewSet;
    private Vector2Int previewIndex;

    
    // ================================
    // Unity Lifecycle
    // ================================

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
    // Initialization
    // ================================

    public void Initialize(BoardGeometry boardGeometry)
    {
        geometry = boardGeometry;

        previewSet = false;
        shadowSprite.enabled = false;
    }


    // ================================
    // Public Methods
    // ================================

    public void SetPreview(Vector2Int index)
    {
        // set the preview
        previewSet = true;
        previewIndex = index;

        shadowSprite.transform.position = geometry.BoardIndexToTransform(index);
        shadowSprite.enabled = true;
    }


    // ================================
    // Event Bus Handlers
    // ================================

    private void OnPlayerDragging(PlayerDraggingEvent e)
    {
        // start the ghost preview
        preview = StartCoroutine(PreviewLoop(e.PlayerTransform, e.Color));
    }

    private void OnPlayerRelease(PlayerReleasedEvent e)
    {
        // stop the ghost preview
        StopCoroutine(preview);
        previewSet = false;
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

            Vector3 position = player.position;
            Vector2Int index = geometry.TransformToBoardIndex(position);

            // try a new preview only when the index changes
            if (previewSet && index != previewIndex)
            {
                ClearPreview();

                TryGhostPreview?.Invoke(index, color);
            }
            else if (!previewSet)
            {
                TryGhostPreview?.Invoke(index, color);
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

        // clear the preview sprite
        shadowSprite.enabled = false;
    }

}
