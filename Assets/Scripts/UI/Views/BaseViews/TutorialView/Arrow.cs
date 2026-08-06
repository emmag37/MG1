using UnityEngine;
using UnityEngine.UI;

// this is basically just a scaler with an index property

public class Arrow : MonoBehaviour
{
    public Vector2Int Index;

    private void Start()
    {
        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        Vector3 worldPos = BoardGeometry.BoardIndexToTransform(Index);

        Scaler.UIApplyScaledPos(transform, canvasRect, worldPos, UIConstants.ArrowOffset);
    }
}
