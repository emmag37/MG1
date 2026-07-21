using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraWidthLock : MonoBehaviour
{
    private const float ReferenceWidth = 1080f;
    private const float ReferenceHeight = 1920f;
    private const float PixelsPerUnit = 100f;

    public float OrthoSize { get; private set; }

    private Camera _cam;

    public void ApplyOrthographicSize()
    {
        _cam = GetComponent<Camera>();

        float referenceAspect = ReferenceWidth / ReferenceHeight;
        float currentAspect = (float)Screen.width / Screen.height;
        float baseOrthoSize = (ReferenceHeight / PixelsPerUnit) / 2f;

        if (currentAspect < referenceAspect)
            _cam.orthographicSize = baseOrthoSize * (referenceAspect / currentAspect);
        else
            _cam.orthographicSize = baseOrthoSize;

        OrthoSize = _cam.orthographicSize;
    }
}
