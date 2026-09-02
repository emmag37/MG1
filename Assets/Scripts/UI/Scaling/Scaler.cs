using UnityEngine;
using System;


public static class Scaler
{
    private static float scale;         // only access for the actual scale, keeps scaling consistent across the project
    private static Camera cam;          // camera that the scaling applies to

    public static void CalculateAndSetScale(Camera c)
    {
        if (c == null)
            throw new ArgumentNullException(nameof(c));
        if (!c.orthographic)
            throw new ArgumentException("[Scalar] Camera must be orthographic to calculate scale");
        if (c.orthographicSize <= 0)
            throw new ArgumentOutOfRangeException("[Scalar] Camera.orthographicSize must be greater than 0");

        cam = c;

        float referenceAspect = UIConstants.ReferenceWidth / UIConstants.ReferenceHeight;
        float currentAspect = (float)Screen.width / Screen.height;
        float baseOrthoSize = (UIConstants.ReferenceHeight / UIConstants.PixelsPerUnit) / 2f;

        if (currentAspect < referenceAspect)
            cam.orthographicSize = baseOrthoSize * (referenceAspect / currentAspect);
        else
            cam.orthographicSize = baseOrthoSize;

        scale = cam.orthographicSize / UIConstants.ReferenceOrtho;
    }

    public static void ApplyLocalScale(Transform transform)
    {
        if (scale == 0)
            throw new InvalidOperationException("[Scaler] Attempted to set value before initializing scale");

        transform.localScale = new Vector3(scale, scale, 1f);
    }

    public static void ApplyScaledYPos(Transform transform)
    {
        if (scale == 0)
            throw new InvalidOperationException("[Scaler] Attempted to set value before initializing scale");

        var pos = transform.position;
        pos.y *= (scale + 1) / 2;   // split the difference
        transform.position = pos;
    }

    public static Vector3 CalculateScaledYPos(Vector3 pos)
    {
        if (scale == 0)
            throw new InvalidOperationException("[Scaler] Attempted to set value before initializing scale");

        pos.y *= (scale + 1) / 2;
        return pos;
    }

    public static void UIApplyScaledPos(Transform transform, RectTransform rect, Vector3 worldPos, float yOffset = 0)
    {
        if (scale == 0)
            throw new InvalidOperationException("[Scaler] Attempted to set value before initializing scale");

        float scaledOffset = scale * yOffset;

        Vector2 screenPos = cam.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            screenPos,
            null, // if overlay
            out Vector2 uIPos
        );

        transform.localPosition = new Vector2(uIPos.x, uIPos.y + scaledOffset);
    }
}
