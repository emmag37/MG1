using UnityEngine;
using System;
using System.Collections.Generic;

public class BaseViewController : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private BaseView[] baseViewList;

    // ==================================================
    // Private Fields
    // ==================================================
    private Dictionary<BaseViewType, BaseView> baseViews = new Dictionary<BaseViewType, BaseView>();
    private BaseView currentView;


    // ==================================================
    // Initializer
    // ==================================================

    public void Initialize()
    {
        foreach (BaseView view in baseViewList)
        {
            if (baseViews.ContainsKey(view.Type))
            {
                Debug.LogError($"Duplicate base view type: {view.Type}");
                continue;
            }

            baseViews.Add(view.Type, view);
        }
    }

    // ==================================================
    // Event Handlers
    // ==================================================

    public void HandleShowView(BaseViewType type, IRuntimeData data)
    {
        ShowView(type, data);
    }

    public void HandleProfileUpdate(IUserSettings userSettings)
    {
        RefreshView(BaseViewType.Home, userSettings);
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void ShowView(BaseViewType type, IRuntimeData data)
    {
        if (currentView != null)
            currentView.Hide();

        currentView = GetBaseView(type);

        currentView.Show(data);

        Debug.Assert(currentView != null, "Current view not set");
        Debug.Assert(currentView.Type == type, $"Show type mismatch. Expected: {type}, Found: {currentView.Type}");
    }

    private void RefreshView(BaseViewType type, IRuntimeData data)
    {
        BaseView view = GetBaseView(type);
        view.UpdateView(data);
    }


    // ==================================================
    // Helper Methods
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
}
