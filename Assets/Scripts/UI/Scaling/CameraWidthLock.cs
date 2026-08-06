using UnityEngine;

// does this have to be monobehaviour??
// what does this even do??
// also, could this become static potentially

// really just adjusts the camera's orthographic size, locking the width

// delete this whole class in favor of the static one
public class CameraWidthLock : MonoBehaviour
{
    public float OrthoSize { get; private set; }    // remove

    private Camera _cam;    // remove

    // have this initialize in bootstrap, then simply call the camera's orthographic size in all other scripts
    public void ApplyOrthographicSize()
    {
        _cam = GetComponent<Camera>();

        float referenceAspect = UIConstants.ReferenceWidth / UIConstants.ReferenceHeight;
        float currentAspect = (float)Screen.width / Screen.height;
        float baseOrthoSize = (UIConstants.ReferenceHeight / UIConstants.PixelsPerUnit) / 2f;

        if (currentAspect < referenceAspect)
            _cam.orthographicSize = baseOrthoSize * (referenceAspect / currentAspect);
        else
            _cam.orthographicSize = baseOrthoSize;

        OrthoSize = _cam.orthographicSize;      // remove
    }
    
}

public static class Scaler
{
    public static float OrthographicWidthLock(Camera cam)
    {
        Debug.Log("apply new camera scale");

        float referenceAspect = UIConstants.ReferenceWidth / UIConstants.ReferenceHeight;
        float currentAspect = (float)Screen.width / Screen.height;
        float baseOrthoSize = (UIConstants.ReferenceHeight / UIConstants.PixelsPerUnit) / 2f;

        if (currentAspect < referenceAspect)
            cam.orthographicSize = baseOrthoSize * (referenceAspect / currentAspect);
        else
            cam.orthographicSize = baseOrthoSize;

        return cam.orthographicSize;
    }

    public static void ApplyLocalScale(Transform transform, float orthoSize)
    {
        float scale = orthoSize / UIConstants.ReferenceOrtho;

        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
