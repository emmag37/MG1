using UnityEngine;

// delete this class

public class SpawnerPositionAdjust : MonoBehaviour
{
    private const float RefOrtho = 9.6f;

    private CameraWidthLock cameraWidthLock;

    
    public Transform Initialize()
    {
        cameraWidthLock = Camera.main.GetComponent<CameraWidthLock>();

        ApplyScale();

        return transform;
    }
    
    
    private void ApplyScale()
    {
        // scale = new_height / old_height
        // have to match this to the camera ortho

        if (cameraWidthLock.OrthoSize == 0) cameraWidthLock.ApplyOrthographicSize();

        float scale = cameraWidthLock.OrthoSize / RefOrtho;

        var pos = transform.position;
        pos.y *= (scale + 1) / 2;   // split the difference
        transform.position = pos;

        Debug.Log($"new transform pos: {pos}");
    }
    
}
