using UnityEngine;
using System;
using System.Collections.Generic;

public class ViewController : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private BaseView[] baseViewList;
    [SerializeField] private PopUpView[] popUpViewList;

    // ==================================================
    // Private Fields
    // ==================================================
    private Dictionary<BaseViewType, BaseView> baseViews = new Dictionary<BaseViewType, BaseView>();
    private Dictionary<PopUpViewType, PopUpView> popUpViews = new Dictionary<PopUpViewType, PopUpView>();

    private BaseView currentView;
    private Stack<PopUpView> overlayStack = new Stack<PopUpView>();


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(baseViewList.Length > 0, "Base view list not initialized");
        Debug.Assert(popUpViewList.Length > 0, "Pop up view list not initialized");

        foreach (BaseView view in baseViewList)
        {
            Debug.Assert(view != null, $"View in base view list is not initialized");
        }

        foreach (PopUpView view in popUpViewList)
        {
            Debug.Assert(view != null, $"View in pop up view list is not initialized");
        }
    }

    void Awake()
    {
        // initialize view dictionaries
        foreach (BaseView view in baseViewList)
        {
            if (baseViews.ContainsKey(view.Type))
            {
                Debug.LogError($"Duplicate base view type: {view.Type}");
                continue;
            }

            baseViews.Add(view.Type, view);
        }

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
    // Public Methods
    // ==================================================

    public void ShowView<T>(BaseViewType type, T data)
    {
        ClearOverlay();
        Debug.Assert(overlayStack.Count == 0, "Overlay stack not empty after clearing");

        if (currentView != null)
            currentView.Hide();

        currentView = GetBaseView(type);

        if (currentView is BaseView<T> typedView)
        {
            typedView.Show(data);
        }
        else
        {
            currentView.Show();
        }

        Debug.Assert(currentView != null, "Current view not set");
        Debug.Assert(currentView.Type == type, $"Show type mismatch. Expected: {type}, Found: {currentView.Type}");
    }

    public void PushOverlay<T>(PopUpViewType type, T data)
    {
        int count = overlayStack.Count;
        if (count > 0)
        {
            Debug.Assert(type != PopUpViewType.Pause && type != PopUpViewType.Profile && type != PopUpViewType.ScoreHistory,
                $"Attempted to push type {type} to a non-empty overlay stack");

            overlayStack.Peek().Hide();
        }

        PopUpView overlayView = GetPopUpView(type);

        if (overlayView is PopUpView<T> typedOverlay)
        {
            typedOverlay.Show(data);
        }
        else
        {
            overlayView.Show();
        }

        overlayStack.Push(overlayView);

        Debug.Assert(count + 1 == overlayStack.Count, "Push did not increase the overlay stack count");
    }

    public PopUpViewType PopOverlay()
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

        return overlayView.Type;
    }

    public PopUpViewType ClearOverlay()
    {
        PopUpViewType finalType = PopUpViewType.None;
        int initialCount = overlayStack.Count;
        
        while (overlayStack.TryPeek(out PopUpView view))
        {
            if (initialCount > 1 && overlayStack.Count == 1) return finalType;  // fall back to base pop-up view

            finalType = PopOverlay();
        }

        return finalType;
    }

    public void RefreshView<T>(BaseViewType type, T data)
    {
        BaseView view = GetBaseView(type);

        if (view is BaseView<T> typedView)
        {
            typedView.UpdateView(data);
        }
    }

    public void RefreshOverlay<T>(PopUpViewType type, T data)
    {
        PopUpView overlayView = GetPopUpView(type);

        if (overlayView is PopUpView<T> typedOverlay)
        {
            typedOverlay.UpdateOverlay(data);
        }
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private BaseView GetBaseView(BaseViewType type)
    {
        BaseView view;
        if (!baseViews.TryGetValue(type, out view))
        {
            Debug.LogError($"Could not access base view for type {type}");
        }

        return view;
    }

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
