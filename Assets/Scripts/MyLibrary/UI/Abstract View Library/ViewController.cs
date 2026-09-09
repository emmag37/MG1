using UnityEngine;
using System;
using System.Collections.Generic;

// todo: add error messages

public class ViewController<TView, TType, TData>
    where TView : UIView<TType>
    where TType : struct, Enum
    where TData : IUIData
{
    // ==================================================
    // Public Fields
    // ==================================================
    public int Count => viewStack.Count;
    
    // ==================================================
    // Private Fields
    // ==================================================
    private Dictionary<TType, TView> viewDictionary;
    private Stack<TView> viewStack;

    private int stackCapacity;

    // ==================================================
    // Constructor
    // ==================================================

    public ViewController(TView[] viewList, int stackCapacity, IUIViewHost host)
    {
        if (viewList == null)
            throw new ArgumentNullException(nameof(viewList));
        if (stackCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(stackCapacity), "[ViewController] Argument must be greater than 0");
        if (host == null)
            throw new ArgumentNullException(nameof(host));

        this.stackCapacity = stackCapacity;

        viewStack = new Stack<TView>(capacity: stackCapacity);
        viewDictionary = new Dictionary<TType, TView>();

        // initialize the dictionary
        foreach (TView view in viewList)
        {
            if (view == null)
            {
                Debug.LogError($"[ViewController] Null view in view list");
                continue;
            }
            if (viewDictionary.ContainsKey(view.Type))
            {
                Debug.LogError($"Duplicate view type: {view.Type}");
                continue;
            }

            view.Initialize(host);
            viewDictionary.Add(view.Type, view);
        }
    }


    // ==================================================
    // Public Methods
    // ==================================================

    // if stack is at capacity, the pushed type replaces the top view
    public void PushView(TType type, TData data)
    {
        TView newView = GetView(type);

        if (viewStack.Count == stackCapacity)   // remove the top most view
        {
            TView topView = viewStack.Pop();
            topView.Hide();
        }

        viewStack.Push(newView);
        newView.Show(data);
    }

    public TType PeekViewType()
    {
        if (viewStack.Count == 0) return default;   // error: peek from empty stack

        return viewStack.Peek().Type;
    }

    public void PopView()
    {
        if (viewStack.Count == 0) return;   // error: pop from empty stack

        TView topView = viewStack.Pop(); 
        topView.Hide();
    }

    public void ClearViews()
    {
        if (viewStack.Count == 0) return;   // error: clear from empty stack

        while (viewStack.Count > 0) PopView();
    }

    public void UpdateView(TType type, TData data)
    {
        TView view = GetView(type);
        view.UpdateView(data);
    }

    // ==================================================
    // Private Methods
    // ==================================================

    private TView GetView(TType type)
    {
        if (!viewDictionary.TryGetValue(type, out TView view))
        {
            Debug.LogError($"Could not access view for type {type}");
        }

        return view;
    }
}
