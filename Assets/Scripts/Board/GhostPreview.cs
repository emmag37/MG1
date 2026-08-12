using UnityEngine;
using System.Collections;
using System;

// strong contender to remove mono behaviour, esp since you have a coroutine runner
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

    private Coroutine preview;
    private bool previewSet;
    private Vector2Int previewIndex;

    
    // ================================
    // Unity Lifecycle
    // ================================

    void OnDestroy()
    {
        EventBus.Unsubscribe<PlayerDraggingEvent>(OnPlayerDragging);
        EventBus.Unsubscribe<PlayerReleasedEvent>(OnPlayerRelease);
    }


    // ================================
    // Initialization
    // ================================

    public void Initialize()
    {
        if (shadowSprite == null)
        {
            Debug.LogError("[GhostPreview] Shadow sprite is uninitialized");
            return;
        }

        previewSet = false;
        shadowSprite.enabled = false;

        EventBus.Subscribe<PlayerDraggingEvent>(OnPlayerDragging);
        EventBus.Subscribe<PlayerReleasedEvent>(OnPlayerRelease);
    }


    // ================================
    // Public Methods
    // ================================

    public void SetPreview(Vector2Int index)
    {
        Debug.Assert(index.x >= 0 && index.x < GameConstants.RowSize &&
            index.y >= 0 && index.y < GameConstants.RowSize,
            $"[GhostPreview] Attempted to set preview to out of bounds index: {index}");

        // set the preview
        previewSet = true;
        previewIndex = index;

        shadowSprite.transform.position = BoardGeometry.BoardIndexToTransform(index);
        shadowSprite.enabled = true;
    }


    // ================================
    // Event Bus Handlers
    // ================================

    private void OnPlayerDragging(PlayerDraggingEvent e)
    {
        Debug.Assert(!previewSet, "[GhostPreview] Preview set before a player was dragging");

        // start the ghost preview
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError("[GhostPreview] Ran ghost preview from inactive object");
            return;
        }
        if (e.PlayerTransform == null)
        {
            Debug.LogError("[GhostPreview] PlayerDraggingEvent had a null PlayerTransform");
            return;
        }
        preview = StartCoroutine(PreviewLoop(e.PlayerTransform, e.Color));
    }

    private void OnPlayerRelease(PlayerReleasedEvent e)
    {
        if (preview != null)
        {
            StopCoroutine(preview);
            preview = null;
            ClearPreview();
        }
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
            Vector2Int index = BoardGeometry.TransformToBoardIndex(position);

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
