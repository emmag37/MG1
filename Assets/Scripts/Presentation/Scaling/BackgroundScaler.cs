using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    private void Start() => ApplyScale();

    private void ApplyScale()
    {
        // scale = new_height / old_height
        float scale = Screen.height / 1920f;

        transform.localScale = new Vector3(
            scale,
            scale,
            1f
        );
    }
}