using UnityEngine;
using System;
using System.Collections.Generic;

public class TutorialViewController : MonoBehaviour
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
    // Public Methods
    // ==================================================

    public void SkipTutorial()
    {
        scenes[currentScene].gameObject.SetActive(false);
        scenes[scenes.Length - 1].gameObject.SetActive(true);
    }

    public void ShowNextStep(int stepCompleted)
    {
        Debug.Assert(stepCompleted == currentScene, "Tutorial step mismatch");

        // update view to the next step
        scenes[currentScene].gameObject.SetActive(false);

        currentScene++;
        scenes[currentScene].gameObject.SetActive(true);
    }

    public void RemoveArrow(Vector2Int index)
    {
        Debug.Log("remove arrow");
        scenes[currentScene].RemoveArrow(index);
    }
}
