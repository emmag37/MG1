using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class ScoreAnimation : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Text pointsText;

    // ==================================================
    // Private Fields
    // ==================================================
    private RectTransform canvasRect;   // parent canvas
    private Image animationImage;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void Awake()
    {
        animationImage = GetComponent<Image>();
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
        transform.localPosition = uiPos;

        StartCoroutine(AnimatePointsRoutine());
    }


    // ==================================================
    // Coroutine
    // ==================================================

    IEnumerator AnimatePointsRoutine()
    {
        animationImage.enabled = true;
        pointsText.enabled = true;

        yield return new WaitForSeconds(0.5f);

        animationImage.enabled = false;
        pointsText.enabled = false;
    }
}
