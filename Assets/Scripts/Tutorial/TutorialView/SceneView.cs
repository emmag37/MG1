using UnityEngine;


public class SceneView : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Arrow[] arrows;                // can be empty


    // ==================================================
    // Public Methods
    // ==================================================

    public void RemoveArrow(Vector2Int index)
    {
        foreach (Arrow arrow in arrows)
        {
            if (arrow.Index == index)
            {
                arrow.gameObject.SetActive(false);
                return;
            }
        }

        Debug.LogWarning($"[SceneView] Could not find arrow to remove for index {index}");
    }

}
