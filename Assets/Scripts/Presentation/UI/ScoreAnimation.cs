using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class ScoreAnimation : MonoBehaviour
{
    // ==================================================
    // Private Fields
    // ==================================================
    private Text pointsText;
    private RectTransform canvasRect;   // parent canvas


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void Awake()
    {
        pointsText = GetComponent<Text>();
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }


    // ==================================================
    // Public Methods
    // ==================================================

    public void AnimateScore(int points, Vector3 worldPos)
    {
        pointsText.text = $"+{points}";

        // update the transform: world -> screen -> UI
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, 
            screenPos,
            null, // if overlay
            out Vector2 uiPos
        );

        pointsText.transform.localPosition = uiPos;

        StartCoroutine(AnimatePointsRoutine());
    }


    // ==================================================
    // Coroutine
    // ==================================================

    IEnumerator AnimatePointsRoutine()
    {
        pointsText.enabled = true;

        yield return new WaitForSeconds(0.5f);

        pointsText.enabled = false;
    }
}
