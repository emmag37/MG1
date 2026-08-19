using UnityEngine;
using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    // uses a dictionary to store the services <interface, implementation>
    private static readonly Dictionary<Type, object> services = new();

    // register a service
    public static void Register<TService>(TService service)
    {
        services.Add(typeof(TService), service);
    }

    // remove a service - true if found and removed, false if not found
    public static void Remove<TService>(TService service)
    {
        services.Remove(typeof(TService));
        
    }

    // get a service
    public static TService Get<TService>()
    {
        if (!services.TryGetValue(typeof(TService), out object foundService))
            throw new InvalidOperationException($"[ServiceLocator] No service registered for {typeof(TService)}");

        return (TService)foundService;
    }
}
