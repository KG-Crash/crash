using System;
using System.Collections.Generic;
using Codice.Client.Common.Connection;
using UnityEngine;

public abstract class UnityTable<T> : ScriptableObject where T : UnityTable<T>
{
    private static Dictionary<Type, ScriptableObject> _dict = new Dictionary<Type, ScriptableObject>();

    public static T Get()
    {
        if (_dict.TryGetValue(typeof(T), out var table) && table != null)
            return table as T;

        _dict[typeof(T)] = CrashResources.LoadUnityTable<T>();
        return _dict[typeof(T)] as T;
    }
}