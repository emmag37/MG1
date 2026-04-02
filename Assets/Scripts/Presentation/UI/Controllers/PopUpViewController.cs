using UnityEngine;
using System;
using System.Collections.Generic;

public class PopUpViewController : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SettingsService settingsService;

    [SerializeField] private PopUpView[] popUpViewList;


    // ==================================================
    // Private Fields
    // ==================================================
    private Dictionary<PopUpViewType, PopUpView> popUpViews = new Dictionary<PopUpViewType, PopUpView>();
    private Stack<PopUpView> overlayStack = new Stack<PopUpView>();


    // ==================================================
    // Unity Lifecycle Methods
    // ==================================================

    // note: set tutorial as tutorial1 so it always launches to the first
    void Awake()
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

    void OnEnable()
    {
        uiManager.PushOverlayView += HandlePush;
        uiManager.PopOverlayView += HandlePop;

        settingsService.ProfileUpdate += HandleProfileUpdate;
    }

    void OnDisable()
    {
        uiManager.PushOverlayView -= HandlePush;
        uiManager.PopOverlayView += HandlePop;

        settingsService.ProfileUpdate -= HandleProfileUpdate;
    }


    // ==================================================
    // Event Handlers
    // ==================================================

    private void HandlePush(PopUpViewType type, object data)
    {
        PushOverlay(type, data);
    }

    private void HandlePop()
    {
        PopOverlay();
    }

    private void HandleProfileUpdate(IUserSettings userSettings)
    {
        RefreshOverlay(PopUpViewType.Profile, userSettings);
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void PushOverlay<T>(PopUpViewType type, T data)
    {
        int count = overlayStack.Count;
        if (count > 0)
        {
            Debug.Assert(type != PopUpViewType.Pause && type != PopUpViewType.Profile && type != PopUpViewType.ScoreHistory,
                $"Attempted to push type {type} to a non-empty overlay stack");

            overlayStack.Peek().Hide();
        }

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

        Debug.Assert(!(overlayView.Type == PopUpViewType.Pause || overlayView.Type == PopUpViewType.Profile
            || overlayView.Type == PopUpViewType.ScoreHistory) || overlayStack.Count == 1,
            "Too many views in overlay stack");

        overlayView.Hide();
        overlayStack.Pop();

        Debug.Assert(count - 1 == overlayStack.Count, "Pop did not decrease the overlay stack count");

        if (overlayStack.Count > 0)
        {
            overlayStack.Peek().Show();
        }
    }

    private void RefreshOverlay<T>(PopUpViewType type, T data)
    {
        PopUpView overlayView = GetPopUpView(type);

        if (overlayView is PopUpView<T> typedOverlay)
        {
            typedOverlay.UpdateOverlay(data);
        }
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
