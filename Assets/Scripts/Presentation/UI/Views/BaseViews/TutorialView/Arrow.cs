using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    public Vector2Int Index => cell.Index;

    [SerializeField] private Cell cell;

    // need to calculate the location based off of the new scale
    private const float RefOrtho = 9.6f;
    private const float RefOffset = 100f;

    private CameraWidthLock cameraWidthLock;

    private void Start()
    {
        cameraWidthLock = Camera.main.GetComponent<CameraWidthLock>();

        ApplyLocation();
    }

    private void ApplyLocation()
    {
        // define y offset for the arrow
        if (cameraWidthLock.OrthoSize == 0) cameraWidthLock.ApplyOrthographicSize();
        float scaledOffset = cameraWidthLock.OrthoSize / RefOrtho * RefOffset;

        // update the transform: world -> screen -> UI
        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        Vector3 worldPos = cell.transform.position;

        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null, // if overlay
            out Vector2 uiPos
        );
        transform.localPosition = new Vector2(uiPos.x, uiPos.y + scaledOffset);
    }

}
