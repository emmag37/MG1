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
    private static float scale;         // only access for the actual scale, keeps scaling consistent across the project

    public static void CalculateAndSetScale(Camera cam)
    {
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
        Debug.Assert(scale != 0);

        transform.localScale = new Vector3(scale, scale, 1f);
    }

    public static void ApplyScaledYPos(Transform transform)
    {
        Debug.Assert(scale != 0);

        var pos = transform.position;
        pos.y *= (scale + 1) / 2;   // split the difference
        transform.position = pos;
    }
}
