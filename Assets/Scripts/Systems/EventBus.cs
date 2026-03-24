using System;
using System.Collections.Generic;

public static class EventBus
{
    // ================================
    // Private Fields
    // ================================
    private static Dictionary<Type, Action<object>> events = new();
    private static Dictionary<Delegate, Action<object>> lookup = new();

    // ================================
    // Public Methods
    // ================================

    public static void Subscribe<T>(Action<T> callback)
    {
        var type = typeof(T);

        Action<object> wrapper = (obj) => callback((T)obj);
        lookup[callback] = wrapper; // store the wrapper so we can remove it later

        if (events.TryGetValue(type, out var action))
        {
            events[type] = action + wrapper;    // add this action to the event listener
        }
        else
        {
            events[type] = wrapper; // initialize event listener with this action
        }
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        var type = typeof(T);

        if (!lookup.TryGetValue(callback, out var wrapper)) // check if this action was saved earlier
        {
            return;
        }

        if (events.TryGetValue(type, out var action))
        {
            action -= wrapper;  // remove this action from the event listener

            if (action == null)
            {
                events.Remove(type);
            }
            else
            {
                events[type] = action;
            }
        }

        lookup.Remove(callback);    // remove this action from saved earlier
    }

    public static void Publish<T>(T evt)
    {
        var type = typeof(T);

        if (events.TryGetValue(type, out var action))
        {
            action.Invoke(evt);   // send event to all listeners
        }
        
    }
    
}
