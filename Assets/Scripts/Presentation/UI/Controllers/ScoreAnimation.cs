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


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void Awake()
    {
        pointsText = GetComponent<Text>();
    }

    void OnEnable()
    {
        EventBus.Subscribe<WinEvent>(OnAnimateScore);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<WinEvent>(OnAnimateScore);
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void OnAnimateScore(WinEvent e)
    {
        pointsText.text = $"+{e.Points}";

        StartCoroutine(AnimatePointsRoutine());
    }

    IEnumerator AnimatePointsRoutine()
    {
        pointsText.enabled = true;

        yield return new WaitForSeconds(0.5f);

        pointsText.enabled = false;
    }
}
