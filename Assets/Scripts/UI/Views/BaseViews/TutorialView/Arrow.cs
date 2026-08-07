using UnityEngine;
using UnityEngine.UI;


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
