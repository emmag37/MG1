using UnityEngine;


public class SceneView : MonoBehaviour
{
    // ================================
    // Inspector Fields
    // ================================
    [SerializeField] private Arrow[] arrows;

    // ================================
    // Public Methods
    // ================================

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
        // if reach here, no arrow was found
    }

}
