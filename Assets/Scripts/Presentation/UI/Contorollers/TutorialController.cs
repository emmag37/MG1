using UnityEngine;
using System;
using System.Collections.Generic;

public class TutorialController : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private UIManager uiManager;
    [SerializeField] private TutorialView[] tutorialViewList;

    // ==================================================
    // Private Fields
    // ==================================================
    private Dictionary<TutorialViewType, TutorialView> tutorialViews = new Dictionary<TutorialViewType, TutorialView>();
    private TutorialView currentView;


    // ==================================================
    // Unity Lifecycle Methods
    // ==================================================

    // note: set tutorial as tutorial1 so it always launches to the first
    void Awake()
    {
        foreach (TutorialView view in tutorialViewList)
        {
            if (tutorialViews.ContainsKey(view.Type))
            {
                Debug.LogError($"Duplicate tutorial view type: {view.Type}");
                continue;
            }

            tutorialViews.Add(view.Type, view);
        }
    }

    void OnEnable()
    {
        uiManager.SwitchTutorialView += HandleSwitchView;
        uiManager.CloseTutorialView += HandleClose;
    }

    void OnDisable()
    {
        uiManager.SwitchTutorialView -= HandleSwitchView;
        uiManager.CloseTutorialView -= HandleClose;
    }


    // ==================================================
    // Event Handlers
    // ==================================================

    private void HandleSwitchView(TutorialViewType type)
    {
        SwitchView(type);
    }

    private void HandleClose()
    {
        currentView.Hide();
        currentView = null;
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void SwitchView(TutorialViewType type)
    {
        if (currentView != null)
            currentView.Hide();

        currentView = GetTutorialView(type);
        currentView.Show();
    }


    // ==================================================
    // Helper Methods
    // ==================================================

    private TutorialView GetTutorialView(TutorialViewType type)
    {
        TutorialView view;
        if (!tutorialViews.TryGetValue(type, out view))
        {
            Debug.LogError($"Could not access tutorial view for type {type}");
        }

        return view;
    }
}
