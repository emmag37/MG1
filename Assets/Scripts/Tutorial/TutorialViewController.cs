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
    // Unity Lifecycle Methods
    // ==================================================

    void OnEnable()
    {
        EventBus.Subscribe<PlacePlayerEvent>(OnPlacePlayer);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<PlacePlayerEvent>(OnPlacePlayer);
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
        Debug.Assert(stepCompleted == currentScene, "Tutorial step mismatch");

        // update view to the next step
        scenes[currentScene].gameObject.SetActive(false);

        currentScene++;
        scenes[currentScene].gameObject.SetActive(true);
    }

    // ==================================================
    // Event Handlers
    // ==================================================

    // function to remove arrow when the player is placed
    private void OnPlacePlayer(PlacePlayerEvent e)
    {
        // remove the arrow associated with the index that was just placed
        scenes[currentScene].RemoveArrow(e.Index);
    }

    

}
