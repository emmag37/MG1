using UnityEngine;
using System;
using System.Collections.Generic;

public class PopUpViewController : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
	[SerializeField] private PopUpView[] popUpViewList;

    // ==================================================
    // Private Fields
    // ==================================================
    private Dictionary<PopUpViewType, PopUpView> popUpViews = new Dictionary<PopUpViewType, PopUpView>();
    private Stack<PopUpView> overlayStack = new Stack<PopUpView>();


    // ==================================================
    // Initialize
    // ==================================================

    public void Initialize()
    {
        foreach (PopUpView view in popUpViewList)
        {
            if (popUpViews.ContainsKey(view.Type))
            {
                Debug.LogError($"Duplicate pop up view type: {view.Type}");
                continue;
            }

            popUpViews.Add(view.Type, view);
        }
    }


    // ==================================================
    // Event Handlers
    // ==================================================

    public void HandlePush(PopUpViewType type, IRuntimeData data)
    {
        PushOverlay(type, data);
    }

    public void HandlePop()
    {
        PopOverlay();
    }

    public void HandleClear()
    {
        ClearOverlay();
    }

    public void HandleProfileUpdate(IUserSettings userSettings)
    {
        RefreshOverlay(PopUpViewType.Profile, userSettings);
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void PushOverlay(PopUpViewType type, IRuntimeData data)
    {
        int count = overlayStack.Count;
        PopUpView overlayView = GetPopUpView(type);

        overlayView.Show(data);
        overlayStack.Push(overlayView);

        Debug.Assert(count + 1 == overlayStack.Count, "Push did not increase the overlay stack count");
    }

    private void PopOverlay()
    {
        int count = overlayStack.Count;
        Debug.Assert(count > 0, "Attempted to pop from empty overlay stack");

        PopUpView overlayView = overlayStack.Peek();

        overlayView.Hide();
        overlayStack.Pop();

        Debug.Assert(count - 1 == overlayStack.Count, "Pop did not decrease the overlay stack count");
    }

    private void ClearOverlay()
    {
        while (overlayStack.Count > 0)
        {
            PopOverlay();
        }
    }

    private void RefreshOverlay(PopUpViewType type, IRuntimeData data)
    {
        PopUpView overlayView = GetPopUpView(type);
        overlayView.UpdateView(data);
    }

    // ==================================================
    // Helper Methods
    // ==================================================

    private PopUpView GetPopUpView(PopUpViewType type)
    {
        PopUpView view;
        if (!popUpViews.TryGetValue(type, out view))
        {
            Debug.LogError($"Could not access pop up view for type {type}");
        }

        return view;
    }
}
