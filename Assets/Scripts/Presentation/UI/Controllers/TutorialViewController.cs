using UnityEngine;
using System;
using System.Collections.Generic;

public class TutorialViewController : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private RectTransform[] scenes;

    // ==================================================
    // Private Fields
    // ==================================================
    private int currentScene;


    // ==================================================
    // Unity Lifecycle Methods
    // ==================================================

    void OnEnable()
    {
        EventBus.Subscribe<TutorialStepCompleteEvent>(OnStepComplete);

        currentScene = 0;
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<TutorialStepCompleteEvent>(OnStepComplete);
    }


    // ==================================================
    // Event Handlers
    // ==================================================

    public void OnStepComplete(TutorialStepCompleteEvent e)
    {
        Debug.Assert(e.StepCompleted == currentScene, "Tutorial step mismatch");

        // update view to the next step
        scenes[currentScene].gameObject.SetActive(false);

        currentScene++;
        scenes[currentScene].gameObject.SetActive(true);
    }

}
