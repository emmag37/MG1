using UnityEngine;

// why is this a monobehaviour?

public class BackgroundScaler : MonoBehaviour
{
    private CameraWidthLock cameraWidthLock;

    private void Start()
    {
        cameraWidthLock = Camera.main.GetComponent<CameraWidthLock>();
        ApplyScale();
    }

    private void ApplyScale()
    {
        // scale = new_height / old_height
        // have to match this to the camera ortho

        if (cameraWidthLock.OrthoSize == 0) cameraWidthLock.ApplyOrthographicSize();

        float scale = cameraWidthLock.OrthoSize / UIConstants.ReferenceOrtho; 
        transform.localScale = new Vector3(
            scale,
            scale,
            1f
        );
    }
}