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
    private Camera cam;
    private RectTransform canvasRect;   // parent canvas
    private Image animationImage;


    // ==================================================
    // Public Methods
    // ==================================================

    public void Initialize()
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("[ScoreAnimation] Main camera not found");
        }

        animationImage = GetComponent<Image>();

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[ScoreAnimation] No Canvas found in parent hierarchy");
            return;
        }
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    public void AnimateScore(int points, Vector3 worldPos)
    {
        pointsText.text = $"+{points}";

        // update the transform: world -> screen -> UI
        Vector2 screenPos = cam.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, 
            screenPos,
            null, // if overlay
            out Vector2 uiPos
        );
        transform.localPosition = uiPos;

        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError("[ScoreAnimation] Called animate score on inactive score animation");
            return;
        }

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
