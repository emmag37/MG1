using UnityEngine;
using System;
using System.Collections.Generic;

// better name for this script would be host
public class TutorialHost : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private SceneView[] scenes;

    // ==================================================
    // Private Fields
    // ==================================================
    private int currentScene = 0;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    public void OnValidate()
    {
        Debug.Assert(scenes.Length > 0, "[TutorialHost] Scenes is empty");
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public void SkipTutorial()
    {
        scenes[currentScene].gameObject.SetActive(false);
        scenes[scenes.Length - 1].gameObject.SetActive(true);
    }

    public void ShowNextStep(int stepCompleted)
    {
        Debug.Assert(stepCompleted == currentScene, "[TutorialHost] Tutorial step mismatch");
        if (stepCompleted + 1 == scenes.Length)
        {
            Debug.LogError("[TutorialHost] Attempted to show step past final scene");
            return;
        }

        // update view to the next step
        scenes[currentScene].gameObject.SetActive(false);

        currentScene++;
        scenes[currentScene].gameObject.SetActive(true);
    }

    public void RemoveArrow(Vector2Int index)
    {
        if (index.x < 0 || index.x >= GameConstants.RowSize || index.y < 0 || index.y > GameConstants.RowSize)
            throw new ArgumentOutOfRangeException($"[TutorialHost] Index {index} out of range");

        scenes[currentScene].RemoveArrow(index);
    }
}
