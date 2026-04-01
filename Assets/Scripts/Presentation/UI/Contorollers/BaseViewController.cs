using UnityEngine;
using System;
using System.Collections.Generic;

public class BaseViewController : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private UIManager uiManager;
    [SerializeField] private BaseView[] baseViewList;

    // ==================================================
    // Private Fields
    // ==================================================
    private Dictionary<BaseViewType, BaseView> baseViews = new Dictionary<BaseViewType, BaseView>();
    private BaseView currentView;


    // ==================================================
    // Unity Lifecycle Methods
    // ==================================================

    void Awake()
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

    void OnEnable()
    {
        EventBus.Subscribe<GameOverEvent>(OnShowGameOver);

        uiManager.ShowBaseView += HandleShowView;
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<GameOverEvent>(OnShowGameOver);

        uiManager.ShowBaseView -= HandleShowView;
    }


    // ==================================================
    // Event Handlers
    // ==================================================

    private void OnShowGameOver(GameOverEvent e)
    {
        // this is only to compile, you need to update the ui views to better pass this data
        // also illustrates the usage of the event so view stays "dumb"
        PlayerProfile profile = new PlayerProfile();
        profile.RecentScore = e.Score;
        profile.HighScore = e.HighScore;

        ShowView(BaseViewType.GameOver, profile);
    }

    private void HandleShowView(BaseViewType type, object data)
    {
        ShowView(type, data);
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void ShowView<T>(BaseViewType type, T data)
    {
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
