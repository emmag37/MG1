using UnityEngine;
using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    // uses a dictionary to store the services <interface, implementation>
    private static readonly Dictionary<Type, object> services = new();

    // register a service - adds new entry if TService hasn't been registered, replaces if it does
    // requires non-null service
    public static void Register<TService>(TService service)
    {
        if (service == null)
            throw new ArgumentNullException(nameof(service));

        services[typeof(TService)] = service;
    }

    // remove a service
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
