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

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 anchorMin = safeArea.position / screenSize;
        Vector2 anchorMax = (safeArea.position + safeArea.size) / screenSize;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;

        Debug.Log("Safe Area: " + Screen.safeArea);
    }
}