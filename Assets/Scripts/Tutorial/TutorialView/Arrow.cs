using UnityEngine;
using UnityEngine.UI;


public class Arrow : MonoBehaviour
{
    // ==================================================
    // Public Fields
    // ==================================================
    public Vector2Int Index;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnValidate()
    {
        Debug.Assert(
            Index.x >= 0 && Index.x < GameConstants.RowSize && Index.y >= 0 && Index.y < GameConstants.RowSize,
            $"[Arrow] Out of bounds index {Index}"
        );
    }

    void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            throw new MissingComponentException("[Arrow] Canvas in parent not found during start");

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector3 worldPos = BoardGeometry.BoardIndexToTransform(Index);

        Scaler.UIApplyScaledPos(transform, canvasRect, worldPos, UIConstants.ArrowOffset);
    }
}
