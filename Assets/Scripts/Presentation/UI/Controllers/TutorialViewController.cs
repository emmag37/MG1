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
    private int currentScene;


    // ==================================================
    // Unity Lifecycle Methods
    // ==================================================

    void OnEnable()
    {
        EventBus.Subscribe<TutorialStepCompleteEvent>(OnStepComplete);
        EventBus.Subscribe<PlacePlayerEvent>(OnPlacePlayer);

        currentScene = 0;
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<TutorialStepCompleteEvent>(OnStepComplete);
        EventBus.Unsubscribe<PlacePlayerEvent>(OnPlacePlayer);
    }


    // ==================================================
    // Event Handlers
    // ==================================================

    public void HandleSkipTutorial()
    {
        Debug.Log("skip tutorial");

        scenes[currentScene].gameObject.SetActive(false);

        scenes[scenes.Length - 1].gameObject.SetActive(true);
    }

    private void OnStepComplete(TutorialStepCompleteEvent e)
    {
        Debug.Assert(e.StepCompleted == currentScene, "Tutorial step mismatch");

        // update view to the next step
        scenes[currentScene].gameObject.SetActive(false);

        currentScene++;
        scenes[currentScene].gameObject.SetActive(true);
    }

    // add function to remove arrow when the player is placed
    private void OnPlacePlayer(PlacePlayerEvent e)
    {
        if (e.PlayerPosition == Vector3.positiveInfinity) return;

        // remove the arrow associated with the index that was just placed
        scenes[currentScene].RemoveArrow(e.Index);
    }

    

}
