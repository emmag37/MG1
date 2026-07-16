using UnityEngine;
using System;
using System.Collections.Generic;

// define a view controller ADT to be used with my ui view subclasses

public class ViewController<TView, TType, TData>
    where TView : UIView<TType>
    where TType : struct, Enum
    where TData : IRuntimeData
{
    // ==================================================
    // Private Fields
    // ==================================================
    private Dictionary<TType, TView> viewDictionary;
    private Stack<TView> viewStack;

    // ==================================================
    // Constructor
    // ==================================================

    public ViewController(TView[] viewList, int stackSize)
    {
        // initialize the dictionary
        viewDictionary = new Dictionary<TType, TView>();

        foreach (TView view in viewList)
        {
            if (viewDictionary.ContainsKey(view.Type))
            {
                Debug.LogError($"Duplicate view type: {view.Type}");
                continue;
            }

            viewDictionary.Add(view.Type, view);
        }

        // initialize the stack
        viewStack = new Stack<TView>(stackSize);
    }
}
