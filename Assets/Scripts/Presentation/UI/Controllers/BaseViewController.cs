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
    // Unity Lifecycle Methods
    // ==================================================

    void OnEnable()
    {
        EventBus.Subscribe<GameOverEvent>(OnShowGameOver);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<GameOverEvent>(OnShowGameOver);
    }


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

    private void OnShowGameOver(GameOverEvent e)
    {
        ShowView(BaseViewType.GameOver, e.ScoreData);
    }

    public void HandleShowView(BaseViewType type, object data)
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

    private void ShowView<T>(BaseViewType type, T data)
    {
        if (currentView != null)
            currentView.Hide();

        currentView = GetBaseView(type);

        currentView.Show(data);

        Debug.Assert(currentView != null, "Current view not set");
        Debug.Assert(currentView.Type == type, $"Show type mismatch. Expected: {type}, Found: {currentView.Type}");
    }

    private void RefreshView<T>(BaseViewType type, T data)
    {
        BaseView view = GetBaseView(type);

        if (view is BaseView<T> typedView)
        {
            typedView.UpdateView(data);
        }
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
