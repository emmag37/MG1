using UnityEngine;

public class SafeAreaPanel : MonoBehaviour
{
    RectTransform rectTransform;
    Rect lastSafeArea;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        if (safeArea == lastSafeArea) return;
        lastSafeArea = safeArea;

        // only apply reduction if device actually has a notch/cutout
        bool hasNotch = safeArea.yMin > 0 || safeArea.yMax < Screen.height
                        || safeArea.xMin > 0 || safeArea.xMax < Screen.width;

        if (hasNotch)
        {
            float topExpansion = 20f;      // increase top inset
            float bottomExpansion = 20f;   // increase bottom inset
            float sideExpansion = 0f;      // increase side insets if needed

            safeArea.y -= bottomExpansion;
            safeArea.height += topExpansion + bottomExpansion;

            safeArea.x -= sideExpansion;
            safeArea.width += sideExpansion * 2;
        }

        // calculate new safe area/anchors
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 anchorMin = safeArea.position / screenSize;
        Vector2 anchorMax = (safeArea.position + safeArea.size) / screenSize;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;

        Debug.Log("Safe Area: " + Screen.safeArea);
    }
}