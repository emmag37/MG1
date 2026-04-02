using UnityEngine;
using System;
using System.Collections.Generic;

public class TutorialController : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private TutorialView[] tutorialViewList;

    // ==================================================
    // Private Fields
    // ==================================================
    private Dictionary<TutorialViewType, TutorialView> tutorialViews = new Dictionary<TutorialViewType, TutorialView>();
    private TutorialView currentView;


    // ==================================================
    // Initialize
    // ==================================================

    public void Initialize()
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


    // ==================================================
    // Event Handlers
    // ==================================================

    public void HandleSwitchView(TutorialViewType type)
    {
        SwitchView(type);
    }

    public void HandleClose()
    {
        currentView.Hide(); // null reference
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
